import React from 'react';
import {
  Box,
  Grid,
  Card,
  CardContent,
  Typography,
  Chip,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  LinearProgress,
} from '@mui/material';
import {
  Users,
  CheckCircle2,
  TrendingUp,
  FileText,
  ShieldCheck,
  Smartphone,
  Download,
  Calendar,
  Layers,
} from 'lucide-react';

export const Dashboard: React.FC = () => {
  const stats = [
    {
      title: 'Active Students',
      value: '1,428',
      change: '+42 this academic year',
      icon: <Users size={20} color="#1B5E3F" />,
    },
    {
      title: 'Tuition Collected',
      value: 'ETB 3,842,500',
      change: '94.2% of billed term total',
      icon: <TrendingUp size={20} color="#E0A526" />,
    },
    {
      title: "Today's Attendance",
      value: '97.4%',
      change: '1,391 Present · 37 Absent',
      icon: <CheckCircle2 size={20} color="#1B5E3F" />,
    },
    {
      title: 'Active CBT Assessment',
      value: 'Grade 10 Biology',
      change: '320 Students Sitting (LAN Sync)',
      icon: <Layers size={20} color="#1B5E3F" />,
    },
  ];

  const recentTransactions = [
    {
      id: 'RCP-2026-0089',
      student: 'Almaz Hailu Bekele',
      grade: 'Grade 9-A',
      amount: 'ETB 4,500.00',
      method: 'Telebirr',
      status: 'Confirmed',
    },
    {
      id: 'RCP-2026-0088',
      student: 'Dawit Yohannes',
      grade: 'Grade 10-B',
      amount: 'ETB 2,250.00',
      method: 'CBE Birr',
      status: 'Confirmed',
    },
    {
      id: 'RCP-2026-0087',
      student: 'Selamawit Tadesse',
      grade: 'Grade 7-C',
      amount: 'ETB 4,500.00',
      method: 'Bank Slip',
      status: 'Confirmed',
    },
  ];

  return (
    <Box sx={{ p: { xs: 2, md: 3 }, maxWidth: 1600, mx: 'auto' }}>
      {/* 1. Page Header & Action Bar */}
      <Box
        sx={{
          display: 'flex',
          flexDirection: { xs: 'column', sm: 'row' },
          justifyContent: 'space-between',
          alignItems: { xs: 'flex-start', sm: 'center' },
          gap: 2,
          mb: 3,
        }}
      >
        <Box>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 0.5 }}>
            <Typography variant="h5" sx={{ fontWeight: 700, color: '#1F2933', fontSize: 22 }}>
              Campus Overview
            </Typography>
            <Chip
              label="Academic Year 2026/27 (Term 1)"
              size="small"
              sx={{
                bgcolor: 'rgba(27, 94, 63, 0.08)',
                color: '#1B5E3F',
                fontWeight: 600,
                fontSize: 12,
              }}
            />
          </Box>
          <Typography variant="body2" sx={{ color: '#6B7280' }}>
            Kera Campus · Real-time operational, academic, and fee collection telemetry.
          </Typography>
        </Box>

        <Box sx={{ display: 'flex', gap: 1.5 }}>
          <Button
            variant="outlined"
            size="small"
            startIcon={<Calendar size={16} />}
            sx={{
              borderColor: '#E5E7EB',
              color: '#4B5563',
              textTransform: 'none',
              fontWeight: 600,
            }}
          >
            Ethiopian: Tikimt 2019
          </Button>
          <Button
            variant="outlined"
            size="small"
            startIcon={<Download size={16} />}
            sx={{
              borderColor: '#E5E7EB',
              color: '#4B5563',
              textTransform: 'none',
              fontWeight: 600,
            }}
          >
            Export Summary
          </Button>
        </Box>
      </Box>

      {/* 2. Primary KPI Stat Cards */}
      <Grid container spacing={2.5} sx={{ mb: 3 }}>
        {stats.map((stat, i) => (
          <Grid item xs={12} sm={6} md={3} key={i}>
            <Card sx={{ height: '100%', borderColor: '#E5E7EB' }}>
              <CardContent sx={{ p: 2.5 }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1.5 }}>
                  <Typography variant="body2" sx={{ color: '#6B7280', fontWeight: 600, fontSize: 13 }}>
                    {stat.title}
                  </Typography>
                  <Box
                    sx={{
                      p: 0.8,
                      borderRadius: 1.5,
                      bgcolor: 'rgba(27, 94, 63, 0.08)',
                      display: 'flex',
                    }}
                  >
                    {stat.icon}
                  </Box>
                </Box>
                <Typography variant="h4" sx={{ fontWeight: 700, fontSize: 24, mb: 0.5, color: '#1F2933' }}>
                  {stat.value}
                </Typography>
                <Typography variant="caption" sx={{ color: '#1B5E3F', fontWeight: 600, fontSize: 12 }}>
                  {stat.change}
                </Typography>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      {/* 3. Main Workspace: Two-Column Layout */}
      <Grid container spacing={3}>
        {/* Left Column: CBT Engine Status & Recent Payments */}
        <Grid item xs={12} lg={8}>
          {/* Active CBT Exam Monitor Banner */}
          <Card sx={{ mb: 3, border: '1px solid #E5E7EB' }}>
            <CardContent sx={{ p: 3 }}>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
                  <Box
                    component="img"
                    src="/assets/icon-exams-assessments.svg"
                    alt="CBT Exams"
                    sx={{ width: 28, height: 28 }}
                  />
                  <Box>
                    <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>
                      Live Computer-Based Test (CBT) Sitting
                    </Typography>
                    <Typography variant="caption" sx={{ color: '#6B7280' }}>
                      Package ID: CBT-G10-BIO-T1 · Local Venue Engine: Active
                    </Typography>
                  </Box>
                </Box>
                <Chip
                  label="100% Offline Resilient"
                  size="small"
                  sx={{
                    bgcolor: 'rgba(27, 94, 63, 0.10)',
                    color: '#1B5E3F',
                    fontWeight: 700,
                  }}
                />
              </Box>

              <Box sx={{ mb: 1.5 }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 0.5 }}>
                  <Typography variant="caption" sx={{ fontWeight: 600, color: '#4B5563' }}>
                    Student Submissions Progress (284 / 320 submitted)
                  </Typography>
                  <Typography variant="caption" sx={{ fontWeight: 700, color: '#1B5E3F' }}>
                    88.7%
                  </Typography>
                </Box>
                <LinearProgress
                  variant="determinate"
                  value={88.7}
                  sx={{
                    height: 8,
                    borderRadius: 4,
                    bgcolor: '#E5E7EB',
                    '& .MuiLinearProgress-bar': { bgcolor: '#1B5E3F' },
                  }}
                />
              </Box>
              <Typography variant="caption" sx={{ color: '#6B7280' }}>
                Continuous auto-save active. Local SQLite WAL buffering student responses. Zero internet dependency.
              </Typography>
            </CardContent>
          </Card>

          {/* Recent Fee Payments Table */}
          <Card sx={{ border: '1px solid #E5E7EB' }}>
            <CardContent sx={{ p: 0 }}>
              <Box sx={{ p: 2.5, display: 'flex', justifyContent: 'space-between', alignItems: 'center', borderBottom: '1px solid #E5E7EB' }}>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
                  <Box
                    component="img"
                    src="/assets/icon-fees-payments.svg"
                    alt="Fees"
                    sx={{ width: 24, height: 24 }}
                  />
                  <Typography variant="subtitle1" sx={{ fontWeight: 700, fontSize: 16 }}>
                    Recent Confirmed Fee Collections
                  </Typography>
                </Box>
                <Button size="small" sx={{ color: '#1B5E3F', textTransform: 'none', fontWeight: 600 }}>
                  View All Invoices →
                </Button>
              </Box>

              <TableContainer component={Paper} elevation={0}>
                <Table size="small">
                  <TableHead sx={{ backgroundColor: '#FAF7F0' }}>
                    <TableRow>
                      <TableCell sx={{ fontWeight: 700, fontSize: 12, color: '#6B7280' }}>Receipt #</TableCell>
                      <TableCell sx={{ fontWeight: 700, fontSize: 12, color: '#6B7280' }}>Student</TableCell>
                      <TableCell sx={{ fontWeight: 700, fontSize: 12, color: '#6B7280' }}>Class</TableCell>
                      <TableCell sx={{ fontWeight: 700, fontSize: 12, color: '#6B7280' }}>Amount</TableCell>
                      <TableCell sx={{ fontWeight: 700, fontSize: 12, color: '#6B7280' }}>Channel</TableCell>
                      <TableCell sx={{ fontWeight: 700, fontSize: 12, color: '#6B7280' }}>Status</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {recentTransactions.map((tx) => (
                      <TableRow key={tx.id} hover>
                        <TableCell sx={{ fontFamily: 'monospace', fontWeight: 600, fontSize: 12 }}>
                          {tx.id}
                        </TableCell>
                        <TableCell sx={{ fontWeight: 600, fontSize: 13 }}>{tx.student}</TableCell>
                        <TableCell sx={{ color: '#6B7280', fontSize: 13 }}>{tx.grade}</TableCell>
                        <TableCell sx={{ fontWeight: 700, fontSize: 13, color: '#1F2933' }}>
                          {tx.amount}
                        </TableCell>
                        <TableCell sx={{ fontSize: 12 }}>{tx.method}</TableCell>
                        <TableCell>
                          <Chip
                            label={tx.status}
                            size="small"
                            sx={{
                              height: 22,
                              fontSize: 11,
                              fontWeight: 700,
                              bgcolor: 'rgba(27, 94, 63, 0.10)',
                              color: '#1B5E3F',
                            }}
                          />
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
            </CardContent>
          </Card>
        </Grid>

        {/* Right Column: Platform Architecture & Security Health */}
        <Grid item xs={12} lg={4}>
          <Card sx={{ mb: 3, border: '1px solid #E5E7EB' }}>
            <CardContent sx={{ p: 2.5 }}>
              <Typography variant="subtitle1" sx={{ fontWeight: 700, mb: 2, fontSize: 15 }}>
                Platform Invariants & Guards
              </Typography>

              <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                <Box sx={{ display: 'flex', alignItems: 'flex-start', gap: 1.5 }}>
                  <ShieldCheck size={20} color="#1B5E3F" style={{ marginTop: 2, flexShrink: 0 }} />
                  <Box>
                    <Typography variant="body2" sx={{ fontWeight: 600 }}>
                      PostgreSQL Kernel RLS
                    </Typography>
                    <Typography variant="caption" sx={{ color: '#6B7280', display: 'block' }}>
                      Enforced via non-superuser session role. Zero cross-tenant leakage.
                    </Typography>
                  </Box>
                </Box>

                <Box sx={{ display: 'flex', alignItems: 'flex-start', gap: 1.5 }}>
                  <TrendingUp size={20} color="#E0A526" style={{ marginTop: 2, flexShrink: 0 }} />
                  <Box>
                    <Typography variant="body2" sx={{ fontWeight: 600 }}>
                      Fee Snapshots & Idempotency
                    </Typography>
                    <Typography variant="caption" sx={{ color: '#6B7280', display: 'block' }}>
                      Student terms protected from retroactive price edits. Immutable posted invoices.
                    </Typography>
                  </Box>
                </Box>

                <Box sx={{ display: 'flex', alignItems: 'flex-start', gap: 1.5 }}>
                  <Smartphone size={20} color="#1B5E3F" style={{ marginTop: 2, flexShrink: 0 }} />
                  <Box>
                    <Typography variant="body2" sx={{ fontWeight: 600 }}>
                      Better Auth Service
                    </Typography>
                    <Typography variant="caption" sx={{ color: '#6B7280', display: 'block' }}>
                      JWT claims mapped to organization tenants and branch permissions.
                    </Typography>
                  </Box>
                </Box>
              </Box>
            </CardContent>
          </Card>

          {/* Quick Shortcuts */}
          <Card sx={{ border: '1px solid #E5E7EB' }}>
            <CardContent sx={{ p: 2.5 }}>
              <Typography variant="subtitle1" sx={{ fontWeight: 700, mb: 1.5, fontSize: 15 }}>
                Quick Module Actions
              </Typography>
              <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
                <Button
                  variant="outlined"
                  fullWidth
                  size="small"
                  startIcon={<Users size={16} />}
                  sx={{ justifyContent: 'flex-start', textTransform: 'none', color: '#1F2933', borderColor: '#E5E7EB' }}
                >
                  Register New Student Admission
                </Button>
                <Button
                  variant="outlined"
                  fullWidth
                  size="small"
                  startIcon={<Layers size={16} />}
                  sx={{ justifyContent: 'flex-start', textTransform: 'none', color: '#1F2933', borderColor: '#E5E7EB' }}
                >
                  Create CBT Assessment Paper
                </Button>
                <Button
                  variant="outlined"
                  fullWidth
                  size="small"
                  startIcon={<FileText size={16} />}
                  sx={{ justifyContent: 'flex-start', textTransform: 'none', color: '#1F2933', borderColor: '#E5E7EB' }}
                >
                  Generate Term Fee Invoices
                </Button>
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
};
