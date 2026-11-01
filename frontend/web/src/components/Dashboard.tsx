import React from 'react';
import {
  Box,
  Grid,
  Card,
  CardContent,
  Typography,
  Chip,
  LinearProgress,
} from '@mui/material';
import {
  Users,
  CheckCircle2,
  TrendingUp,
  Database,
  ShieldCheck,
  Smartphone,
} from 'lucide-react';

export const Dashboard: React.FC = () => {
  const stats = [
    {
      title: 'Active Students',
      value: '1,428',
      change: '+42 this term',
      icon: <Users size={24} color="#1B5E3F" />,
      color: '#1B5E3F',
    },
    {
      title: 'Tuition Collected',
      value: 'ETB 3,842,500',
      change: '94% of billed total',
      icon: <TrendingUp size={24} color="#E0A526" />,
      color: '#E0A526',
    },
    {
      title: 'Today Attendance',
      value: '97.2%',
      change: '1,388 present',
      icon: <CheckCircle2 size={24} color="#1B5E3F" />,
      color: '#1B5E3F',
    },
  ];

  return (
    <Box sx={{ p: 4, maxWidth: 1400, mx: 'auto' }}>
      {/* Welcome Banner */}
      <Box
        sx={{
          mb: 4,
          p: 3.5,
          borderRadius: 3,
          background: 'linear-gradient(135deg, #1B5E3F 0%, #12422C 100%)',
          color: '#FFFFFF',
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          boxShadow: '0 4px 12px rgba(27, 94, 63, 0.2)',
        }}
      >
        <Box>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, mb: 1 }}>
            <Typography variant="h5" sx={{ fontWeight: 700, color: '#FFFFFF' }}>
              Welcome to Fidel Hub
            </Typography>
            <Chip
              label="Tenant: St. Joseph Academy"
              size="small"
              sx={{
                bgcolor: '#E0A526',
                color: '#1F2933',
                fontWeight: 700,
              }}
            />
          </Box>
          <Typography variant="body1" sx={{ color: 'rgba(255, 255, 255, 0.85)' }}>
            Domain-driven modular monolith with PostgreSQL Row-Level Security, fee invoice integrity, and offline CBT exams.
          </Typography>
        </Box>
        <Box
          component="img"
          src="/assets/fidel-hub-symbol.svg"
          alt="ፊ Fidel Hub Symbol"
          sx={{ height: 64, opacity: 0.95 }}
        />
      </Box>

      {/* Primary KPI Cards */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        {stats.map((stat, i) => (
          <Grid item xs={12} sm={6} md={4} key={i}>
            <Card>
              <CardContent sx={{ p: 3 }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
                  <Typography variant="body2" sx={{ color: '#6B7280', fontWeight: 600 }}>
                    {stat.title}
                  </Typography>
                  <Box
                    sx={{
                      p: 1,
                      borderRadius: 2,
                      bgcolor: 'rgba(27, 94, 63, 0.08)',
                      display: 'flex',
                    }}
                  >
                    {stat.icon}
                  </Box>
                </Box>
                <Typography variant="h4" sx={{ fontWeight: 700, mb: 1 }}>
                  {stat.value}
                </Typography>
                <Typography variant="caption" sx={{ color: '#1B5E3F', fontWeight: 600 }}>
                  {stat.change}
                </Typography>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      {/* Architectural Health & Invariants */}
      <Typography variant="h6" sx={{ mb: 2, fontWeight: 700 }}>
        Platform Architecture & Invariants Status
      </Typography>
      <Grid container spacing={3}>
        <Grid item xs={12} md={4}>
          <Card sx={{ height: '100%' }}>
            <CardContent sx={{ p: 3 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, mb: 2 }}>
                <ShieldCheck size={22} color="#1B5E3F" />
                <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>
                  Tenant Isolation
                </Typography>
              </Box>
              <Typography variant="body2" sx={{ color: '#4B5563', mb: 2 }}>
                Enforced via PostgreSQL Row-Level Security (RLS) with composite primary keys `(tenant_id, id)`.
              </Typography>
              <Chip label="RLS Active" color="success" size="small" />
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} md={4}>
          <Card sx={{ height: '100%' }}>
            <CardContent sx={{ p: 3 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, mb: 2 }}>
                <Database size={22} color="#E0A526" />
                <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>
                  Fee & Payment Integrity
                </Typography>
              </Box>
              <Typography variant="body2" sx={{ color: '#4B5563', mb: 2 }}>
                Posted invoice immutability, historical fee snapshots, and idempotency protection on payment receipts.
              </Typography>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                <Chip label="Idempotency & Snapshots Active" color="success" size="small" />
              </Box>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} md={4}>
          <Card sx={{ height: '100%' }}>
            <CardContent sx={{ p: 3 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, mb: 2 }}>
                <Smartphone size={22} color="#1B5E3F" />
                <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>
                  Offline CBT Exam Engine
                </Typography>
              </Box>
              <Typography variant="body2" sx={{ color: '#4B5563', mb: 1 }}>
                Encrypted exam package download, student progress auto-save, and idempotent test submission sync.
              </Typography>
              <Box sx={{ mt: 2 }}>
                <Typography variant="caption" sx={{ color: '#6B7280' }}>
                  CBT sync worker health
                </Typography>
                <LinearProgress variant="determinate" value={100} sx={{ height: 6, borderRadius: 3, mt: 0.5, bgcolor: '#E5E7EB', '& .MuiLinearProgress-bar': { bgcolor: '#1B5E3F' } }} />
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
};
