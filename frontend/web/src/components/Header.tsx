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
  InputBase,
  Button,
} from '@mui/material';
import {
  Bell,
  Search,
  School,
  MapPin,
  Wifi,
  Plus,
} from 'lucide-react';

interface HeaderProps {
  tenantName?: string;
  branchName?: string;
  userName?: string;
}

export const Header: React.FC<HeaderProps> = ({
  tenantName = 'Fidel Hub Academy',
  branchName = 'Kera Campus',
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
        zIndex: 1100,
      }}
    >
      <Toolbar sx={{ minHeight: 64, px: 3, display: 'flex', justifyContent: 'space-between', gap: 2 }}>
        {/* Left: Brand Logo & Organization Context */}
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
          <Box
            component="img"
            src="/assets/fidel-hub-primary-logo.svg"
            alt="Fidel Hub"
            sx={{ height: 38, cursor: 'pointer' }}
          />

          <Box sx={{ display: { xs: 'none', md: 'flex' }, alignItems: 'center', gap: 1 }}>
            <Chip
              icon={<School size={15} color="#1B5E3F" />}
              label={tenantName}
              size="small"
              sx={{
                backgroundColor: 'rgba(27, 94, 63, 0.08)',
                borderColor: '#1B5E3F',
                fontWeight: 600,
                color: '#1B5E3F',
                height: 28,
              }}
            />

            <Chip
              icon={<MapPin size={14} color="#6B7280" />}
              label={branchName}
              size="small"
              variant="outlined"
              sx={{
                borderColor: '#E5E7EB',
                color: '#4B5563',
                fontWeight: 500,
                height: 28,
              }}
            />
          </Box>
        </Box>

        {/* Center: Command Palette / Global Search */}
        <Box
          sx={{
            display: { xs: 'none', lg: 'flex' },
            alignItems: 'center',
            backgroundColor: '#FAF7F0',
            border: '1px solid #E5E7EB',
            borderRadius: 2,
            px: 1.5,
            py: 0.5,
            width: 380,
            maxWidth: '100%',
          }}
        >
          <Search size={16} color="#6B7280" style={{ marginRight: 8 }} />
          <InputBase
            placeholder="Search students, admission numbers, exams... (Ctrl+K)"
            sx={{ fontSize: 13, flex: 1, color: '#1F2933' }}
          />
          <Chip
            label="Ctrl+K"
            size="small"
            sx={{
              height: 20,
              fontSize: 10,
              fontWeight: 700,
              bgcolor: '#FFFFFF',
              border: '1px solid #E5E7EB',
              color: '#6B7280',
            }}
          />
        </Box>

        {/* Right: Actions, Sync Status & User Profile */}
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
          {/* Offline/Online Resilient Sync Pill */}
          <Chip
            icon={<Wifi size={14} color="#1B5E3F" />}
            label="Online · Synced"
            size="small"
            sx={{
              backgroundColor: 'rgba(27, 94, 63, 0.10)',
              color: '#1B5E3F',
              fontWeight: 600,
              fontSize: 12,
              height: 28,
              display: { xs: 'none', sm: 'inline-flex' },
            }}
          />

          <Button
            variant="contained"
            size="small"
            startIcon={<Plus size={16} />}
            sx={{
              backgroundColor: '#1B5E3F',
              fontWeight: 600,
              height: 32,
              fontSize: 13,
              display: { xs: 'none', sm: 'inline-flex' },
              '&:hover': { backgroundColor: '#12422C' },
            }}
          >
            New
          </Button>

          <IconButton size="small" sx={{ color: '#1F2933' }}>
            <Badge badgeContent={2} color="secondary">
              <Bell size={18} />
            </Badge>
          </IconButton>

          {/* User Avatar */}
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.2, ml: 0.5 }}>
            <Avatar
              sx={{
                width: 34,
                height: 34,
                bgcolor: '#1B5E3F',
                fontSize: 13,
                fontWeight: 700,
              }}
            >
              PB
            </Avatar>
            <Box sx={{ display: { xs: 'none', xl: 'block' } }}>
              <Typography variant="body2" sx={{ fontWeight: 600, lineHeight: 1.2, fontSize: 13 }}>
                {userName}
              </Typography>
              <Typography variant="caption" sx={{ color: '#6B7280', fontSize: 11 }}>
                Campus Director
              </Typography>
            </Box>
          </Box>
        </Box>
      </Toolbar>
    </AppBar>
  );
};
