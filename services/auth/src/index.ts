/**
 * Fidel Hub Auth Service — Sprint 1 Identity Test-Double (Better Auth Mock Harness)
 * Provides local RS256 token issuance and JWKS discovery metadata for ASP.NET Core API validation.
 */
import express, { Request, Response } from 'express';
import cors from 'cors';
import dotenv from 'dotenv';
import { SignJWT, exportJWK, generateKeyPair } from 'jose';

dotenv.config();

const app = express();
const PORT = process.env.PORT || 4000;
const PUBLIC_URL = process.env.BETTER_AUTH_URL || process.env.AUTH_PUBLIC_URL || `http://localhost:${PORT}`;
const ISSUER = process.env.BETTER_AUTH_ISSUER || PUBLIC_URL;
const AUDIENCE = process.env.BETTER_AUTH_AUDIENCE || 'fidel-hub-api';

if (process.env.NODE_ENV === 'production') {
  throw new Error('FATAL: The mock auth test-double cannot be run in production. Deploy the persistent Better Auth microservice with database authentication.');
}

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
    service: 'Fidel Hub Better Auth Mock Service',
    timestamp: new Date().toISOString(),
  });
});

// 2. OIDC discovery & JWKS endpoints for ASP.NET Core JwtBearer validation
app.get('/.well-known/openid-configuration', (_req: Request, res: Response) => {
  res.json({
    issuer: ISSUER,
    jwks_uri: `${PUBLIC_URL}/.well-known/jwks.json`,
    id_token_signing_alg_values_supported: ['RS256'],
  });
});

// JWKS endpoint
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

  // Baseline credential check for development test double
  const validUsers: Record<string, { id: string; name: string; password: string }> = {
    'solomon@fidelhub.com': {
      id: 'b2f6c0a8-4e12-4d56-9812-7a541289de01',
      name: 'Solomon Bekele Hailu',
      password: process.env.DEMO_USER_PASSWORD || 'Password123!',
    },
    'admin@fidelhub.com': {
      id: 'b2f6c0a8-4e12-4d56-9812-7a541289de01',
      name: 'Platform Administrator',
      password: process.env.DEMO_USER_PASSWORD || 'Password123!',
    },
  };

  const normalizedEmail = email.toLowerCase().trim();
  const matchedUser = validUsers[normalizedEmail];

  if (!matchedUser || password !== matchedUser.password) {
    return res.status(401).json({ error: 'Invalid email or password.' });
  }

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
    sub: matchedUser.id,
    email: normalizedEmail,
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
      id: matchedUser.id,
      email: normalizedEmail,
      fullName: matchedUser.name,
      tenantId: demoTenantId,
      branchId: activeBranchId,
      roles,
      permissions,
    },
  });
});

initKeys().then(() => {
  app.listen(PORT, () => {
    console.log(`[Fidel Hub Auth] Better Auth mock identity test harness listening on port ${PORT}`);
    console.log(`[Fidel Hub Auth] JWKS public key endpoint: ${PUBLIC_URL}/.well-known/jwks.json`);
  });
});
