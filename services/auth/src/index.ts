import express, { Request, Response } from 'express';
import cors from 'cors';
import dotenv from 'dotenv';
import { SignJWT, exportJWK, generateKeyPair } from 'jose';

dotenv.config();

const app = express();
const PORT = process.env.PORT || 4000;
const ISSUER = process.env.BETTER_AUTH_ISSUER || 'https://auth.fidelhub.com';
const AUDIENCE = process.env.BETTER_AUTH_AUDIENCE || 'fidel-hub-api';

app.use(cors());
app.use(express.json());

// In-memory cryptographic key pair for local JWT issuance and verification
let privateKey: any;
let publicKeyJWK: any;

async function initKeys() {
  const keyPair = await generateKeyPair('RS256', { modulusLength: 2048 });
  privateKey = keyPair.privateKey;
  publicKeyJWK = await exportJWK(keyPair.publicKey);
  publicKeyJWK.kid = 'fidel-hub-auth-key-1';
  publicKeyJWK.use = 'sig';
  publicKeyJWK.alg = 'RS256';
}

// 1. Health check probe
app.get('/health', (_req: Request, res: Response) => {
  res.json({
    status: 'Healthy',
    service: 'Fidel Hub Better Auth Service',
    timestamp: new Date().toISOString(),
  });
});

// 2. JWKS endpoint for ASP.NET Core JwtBearer validation
app.get('/.well-known/jwks.json', (_req: Request, res: Response) => {
  res.json({
    keys: [publicKeyJWK],
  });
});

// 3. User Authentication & Multi-Tenant JWT Token Issuance
app.post('/api/auth/sign-in', async (req: Request, res: Response) => {
  const { email, password, branchId } = req.body;

  if (!email || !password) {
    return res.status(400).json({ error: 'Email and password are required.' });
  }

  // Demo user authentication resolution for Sprint 1
  const demoUserId = 'b2f6c0a8-4e12-4d56-9812-7a541289de01';
  const demoTenantId = '11111111-1111-1111-1111-111111111111'; // Fidel Hub Academy
  const activeBranchId = branchId || '22222222-2222-2222-2222-222222222222'; // Main Bole Campus

  const roles = ['TEACHER', 'EXAM_COORDINATOR'];
  const permissions = [
    'att.session.take',
    'grade.marksheet.enter',
    'exam.question.create',
    'exam.cbt.package.export',
    'lib.loan.checkout',
  ];

  const jwt = await new SignJWT({
    sub: demoUserId,
    email: email.toLowerCase(),
    tenant_id: demoTenantId,
    branch_id: activeBranchId,
    roles,
    permissions,
  })
    .setProtectedHeader({ alg: 'RS256', kid: publicKeyJWK.kid })
    .setIssuedAt()
    .setIssuer(ISSUER)
    .setAudience(AUDIENCE)
    .setExpirationTime('24h')
    .sign(privateKey);

  return res.json({
    token: jwt,
    user: {
      id: demoUserId,
      email,
      fullName: 'Solomon Bekele Hailu',
      tenantId: demoTenantId,
      branchId: activeBranchId,
      roles,
      permissions,
    },
  });
});

initKeys().then(() => {
  app.listen(PORT, () => {
    console.log(`[Fidel Hub Auth] Dedicated Better Auth identity service listening on port ${PORT}`);
    console.log(`[Fidel Hub Auth] JWKS public key endpoint: http://localhost:${PORT}/.well-known/jwks.json`);
  });
});
