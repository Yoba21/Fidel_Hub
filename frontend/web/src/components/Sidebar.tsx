import React from 'react';
import {
  Box,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Typography,
  Divider,
} from '@mui/material';
import {
  LayoutDashboard,
  Users,
  GraduationCap,
  CalendarCheck,
  Settings,
} from 'lucide-react';

interface SidebarProps {
  activeTab: string;
  onTabChange: (tabId: string) => void;
}

export const Sidebar: React.FC<SidebarProps> = ({ activeTab, onTabChange }) => {
  const mainNav = [
    { id: 'dashboard', label: 'Dashboard', icon: <LayoutDashboard size={20} /> },
    { id: 'students', label: 'Students & Guardians', icon: <Users size={20} /> },
    { id: 'academics', label: 'Academic Setup', icon: <GraduationCap size={20} /> },
    { id: 'attendance', label: 'Attendance', icon: <CalendarCheck size={20} /> },
  ];

  const brandModuleNav = [
    {
      id: 'fees',
      label: 'Fees & Ledger',
      iconUrl: '/assets/icon-fees-payments.svg',
      badge: 'Balanced',
    },
    {
      id: 'exams',
      label: 'Exams & Grading',
      iconUrl: '/assets/icon-exams-assessments.svg',
    },
    {
      id: 'reports',
      label: 'Report Cards',
      iconUrl: '/assets/icon-report-cards.svg',
    },
    {
      id: 'updates',
      label: 'Parent Updates',
      iconUrl: '/assets/icon-parent-updates.svg',
    },
  ];

  return (
    <Box
      sx={{
        width: 260,
        flexShrink: 0,
        backgroundColor: '#FFFFFF',
        borderRight: '1px solid #E5E7EB',
        display: 'flex',
        flexDirection: 'column',
        minHeight: 'calc(100vh - 70px)',
      }}
    >
      <Box sx={{ p: 2 }}>
        <Typography
          variant="overline"
          sx={{ color: '#6B7280', fontWeight: 700, letterSpacing: 1 }}
        >
          Core School
        </Typography>
        <List dense sx={{ mt: 0.5 }}>
          {mainNav.map((item) => (
            <ListItem key={item.id} disablePadding sx={{ mb: 0.5 }}>
              <ListItemButton
                selected={activeTab === item.id}
                onClick={() => onTabChange(item.id)}
                sx={{
                  borderRadius: 2,
                  '&.Mui-selected': {
                    backgroundColor: 'rgba(27, 94, 63, 0.08)',
                    color: '#1B5E3F',
                    fontWeight: 600,
                    '& .MuiListItemIcon-root': { color: '#1B5E3F' },
                  },
                }}
              >
                <ListItemIcon sx={{ minWidth: 36, color: '#4B5563' }}>
                  {item.icon}
                </ListItemIcon>
                <ListItemText
                  primary={item.label}
                  primaryTypographyProps={{ fontSize: 14, fontWeight: activeTab === item.id ? 600 : 500 }}
                />
              </ListItemButton>
            </ListItem>
          ))}
        </List>
      </Box>

      <Divider sx={{ my: 1 }} />

      <Box sx={{ p: 2 }}>
        <Typography
          variant="overline"
          sx={{ color: '#6B7280', fontWeight: 700, letterSpacing: 1 }}
        >
          Specialized Modules
        </Typography>
        <List dense sx={{ mt: 0.5 }}>
          {brandModuleNav.map((item) => (
            <ListItem key={item.id} disablePadding sx={{ mb: 0.5 }}>
              <ListItemButton
                selected={activeTab === item.id}
                onClick={() => onTabChange(item.id)}
                sx={{
                  borderRadius: 2,
                  '&.Mui-selected': {
                    backgroundColor: 'rgba(27, 94, 63, 0.08)',
                    color: '#1B5E3F',
                    fontWeight: 600,
                  },
                }}
              >
                <ListItemIcon sx={{ minWidth: 36 }}>
                  <Box
                    component="img"
                    src={item.iconUrl}
                    alt={item.label}
                    sx={{ width: 22, height: 22 }}
                  />
                </ListItemIcon>
                <ListItemText
                  primary={item.label}
                  primaryTypographyProps={{ fontSize: 14, fontWeight: activeTab === item.id ? 600 : 500 }}
                />
              </ListItemButton>
            </ListItem>
          ))}
        </List>
      </Box>

      <Box sx={{ mt: 'auto', p: 2 }}>
        <Divider sx={{ mb: 1.5 }} />
        <ListItemButton
          onClick={() => onTabChange('settings')}
          sx={{ borderRadius: 2 }}
        >
          <ListItemIcon sx={{ minWidth: 36, color: '#4B5563' }}>
            <Settings size={20} />
          </ListItemIcon>
          <ListItemText primary="School Settings" primaryTypographyProps={{ fontSize: 14 }} />
        </ListItemButton>
      </Box>
    </Box>
  );
};
