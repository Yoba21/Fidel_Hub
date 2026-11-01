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
  HelpCircle,
  FileQuestion,
} from 'lucide-react';

interface SidebarProps {
  activeTab: string;
  onTabChange: (tabId: string) => void;
}

export const Sidebar: React.FC<SidebarProps> = ({ activeTab, onTabChange }) => {
  const sections = [
    {
      title: 'Core Operations',
      items: [
        { id: 'dashboard', label: 'Dashboard', icon: <LayoutDashboard size={18} /> },
        { id: 'students', label: 'Students & SIS', icon: <Users size={18} /> },
        { id: 'academics', label: 'Academics & Classes', icon: <GraduationCap size={18} /> },
        { id: 'attendance', label: 'Attendance', icon: <CalendarCheck size={18} /> },
      ],
    },
    {
      title: 'Examinations & CBT',
      items: [
        { id: 'question-bank', label: 'Question Bank', icon: <FileQuestion size={18} /> },
        {
          id: 'exams',
          label: 'Exams & CBT Runner',
          iconUrl: '/assets/icon-exams-assessments.svg',
        },
        {
          id: 'reports',
          label: 'Report Cards',
          iconUrl: '/assets/icon-report-cards.svg',
        },
      ],
    },
    {
      title: 'Finance & Administration',
      items: [
        {
          id: 'fees',
          label: 'Fees & Invoicing',
          iconUrl: '/assets/icon-fees-payments.svg',
        },
        {
          id: 'updates',
          label: 'Parent Alerts (SMS)',
          iconUrl: '/assets/icon-parent-updates.svg',
        },
      ],
    },
  ];

  return (
    <Box
      sx={{
        width: 250,
        flexShrink: 0,
        backgroundColor: '#FFFFFF',
        borderRight: '1px solid #E5E7EB',
        display: 'flex',
        flexDirection: 'column',
        minHeight: 'calc(100vh - 64px)',
      }}
    >
      <Box sx={{ p: 2, flex: 1, overflowY: 'auto' }}>
        {sections.map((sec, secIdx) => (
          <Box key={sec.title} sx={{ mb: 2 }}>
            <Typography
              variant="overline"
              sx={{
                color: '#6B7280',
                fontWeight: 700,
                fontSize: 11,
                letterSpacing: 0.8,
                px: 1,
                display: 'block',
              }}
            >
              {sec.title}
            </Typography>
            <List dense sx={{ mt: 0.5, p: 0 }}>
              {sec.items.map((item) => {
                const isSelected = activeTab === item.id;
                return (
                  <ListItem key={item.id} disablePadding sx={{ mb: 0.3 }}>
                    <ListItemButton
                      selected={isSelected}
                      onClick={() => onTabChange(item.id)}
                      sx={{
                        borderRadius: 1.5,
                        py: 0.8,
                        px: 1.2,
                        '&.Mui-selected': {
                          backgroundColor: 'rgba(27, 94, 63, 0.08)',
                          color: '#1B5E3F',
                          fontWeight: 600,
                          '& .MuiListItemIcon-root': { color: '#1B5E3F' },
                        },
                        '&:hover': {
                          backgroundColor: 'rgba(27, 94, 63, 0.04)',
                        },
                      }}
                    >
                      <ListItemIcon sx={{ minWidth: 32, color: '#4B5563' }}>
                        {item.icon ? (
                          item.icon
                        ) : (
                          <Box
                            component="img"
                            src={item.iconUrl}
                            alt={item.label}
                            sx={{ width: 18, height: 18 }}
                          />
                        )}
                      </ListItemIcon>
                      <ListItemText
                        primary={item.label}
                        primaryTypographyProps={{
                          fontSize: 13,
                          fontWeight: isSelected ? 600 : 500,
                        }}
                      />
                    </ListItemButton>
                  </ListItem>
                );
              })}
            </List>
            {secIdx < sections.length - 1 && <Divider sx={{ my: 1.5 }} />}
          </Box>
        ))}
      </Box>

      {/* Footer System Navigation */}
      <Box sx={{ p: 1.5, borderTop: '1px solid #E5E7EB', backgroundColor: '#FAF7F0' }}>
        <ListItemButton
          onClick={() => onTabChange('settings')}
          sx={{ borderRadius: 1.5, py: 0.6 }}
        >
          <ListItemIcon sx={{ minWidth: 30, color: '#6B7280' }}>
            <Settings size={17} />
          </ListItemIcon>
          <ListItemText primary="Campus Settings" primaryTypographyProps={{ fontSize: 13 }} />
        </ListItemButton>

        <ListItemButton
          onClick={() => onTabChange('help')}
          sx={{ borderRadius: 1.5, py: 0.6 }}
        >
          <ListItemIcon sx={{ minWidth: 30, color: '#6B7280' }}>
            <HelpCircle size={17} />
          </ListItemIcon>
          <ListItemText primary="Help & Guides" primaryTypographyProps={{ fontSize: 13 }} />
        </ListItemButton>
      </Box>
    </Box>
  );
};
