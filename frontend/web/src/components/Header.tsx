import React from 'react';
import {
  AppBar,
  Toolbar,
  Typography,
  Box,
  IconButton,
  Chip,
  Avatar,
  Badge,
} from '@mui/material';
import { Bell, Search, School } from 'lucide-react';

interface HeaderProps {
  tenantName?: string;
  userName?: string;
}

export const Header: React.FC<HeaderProps> = ({
  tenantName = 'Fidel Hub Academy',
  userName = 'Principal Bekele',
}) => {
  return (
    <AppBar
      position="sticky"
      elevation={0}
      sx={{
        backgroundColor: '#FFFFFF',
        borderBottom: '1px solid #E5E7EB',
        color: '#1F2933',
      }}
    >
      <Toolbar sx={{ minHeight: 70, px: 3, display: 'flex', justifyContent: 'space-between' }}>
        {/* Left: Brand Logo & Tenant Chip */}
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2.5 }}>
          <Box
            component="img"
            src="/assets/fidel-hub-primary-logo.svg"
            alt="Fidel Hub"
            sx={{ height: 42, cursor: 'pointer' }}
          />

          <Chip
            icon={<School size={16} color="#1B5E3F" />}
            label={tenantName}
            size="small"
            sx={{
              backgroundColor: '#FAF7F0',
              borderColor: '#1B5E3F',
              borderWidth: 1,
              borderStyle: 'solid',
              fontWeight: 600,
              color: '#1B5E3F',
              px: 0.5,
            }}
          />
        </Box>

        {/* Right: Actions & User profile */}
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
          <IconButton size="small" sx={{ color: '#1F2933' }}>
            <Search size={20} />
          </IconButton>

          <IconButton size="small" sx={{ color: '#1F2933' }}>
            <Badge badgeContent={3} color="secondary">
              <Bell size={20} />
            </Badge>
          </IconButton>

          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, ml: 1 }}>
            <Avatar
              sx={{
                width: 36,
                height: 36,
                bgcolor: '#1B5E3F',
                fontSize: 14,
                fontWeight: 600,
              }}
            >
              PB
            </Avatar>
            <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
              <Typography variant="body2" sx={{ fontWeight: 600, lineHeight: 1.2 }}>
                {userName}
              </Typography>
              <Typography variant="caption" sx={{ color: '#4B5563' }}>
                Administrator
              </Typography>
            </Box>
          </Box>
        </Box>
      </Toolbar>
    </AppBar>
  );
};
