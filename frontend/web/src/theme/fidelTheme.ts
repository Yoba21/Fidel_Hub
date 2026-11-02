import { createTheme } from '@mui/material/styles';

/**
 * Fidel Hub Enterprise Design System
 * Modern, high-trust institutional palette replacing legacy yellow/cream tints
 * with crisp slate surfaces, executive emerald, and high-contrast typography.
 */
export const fidelTheme = createTheme({
  palette: {
    primary: {
      main: '#0F5132', // Deep Executive Emerald
      light: '#1B7348',
      dark: '#0A3B24',
      contrastText: '#FFFFFF',
    },
    secondary: {
      main: '#0F172A', // Slate Navy / High-trust neutral
      light: '#334155',
      dark: '#020617',
      contrastText: '#FFFFFF',
    },
    text: {
      primary: '#0F172A', // Slate 900
      secondary: '#475569', // Slate 600
    },
    background: {
      default: '#F8FAFC', // Slate 50 (Clean, crisp enterprise canvas)
      paper: '#FFFFFF',
    },
    divider: '#E2E8F0', // Slate 200
  },
  typography: {
    fontFamily: [
      'Inter',
      '"Noto Sans Ethiopic"',
      '-apple-system',
      'BlinkMacSystemFont',
      '"Segoe UI"',
      'Roboto',
      'sans-serif',
    ].join(','),
    h4: {
      fontWeight: 700,
      color: '#0F172A',
    },
    h5: {
      fontWeight: 600,
      color: '#0F172A',
    },
    h6: {
      fontWeight: 600,
      color: '#0F172A',
    },
    subtitle1: {
      color: '#475569',
    },
    button: {
      textTransform: 'none',
      fontWeight: 600,
    },
  },
  shape: {
    borderRadius: 8,
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 8,
          boxShadow: 'none',
          textTransform: 'none',
          fontWeight: 600,
          '&:hover': {
            boxShadow: '0 1px 3px rgba(15, 23, 42, 0.1)',
          },
        },
        containedPrimary: {
          backgroundColor: '#0F5132',
          '&:hover': {
            backgroundColor: '#0A3B24',
          },
        },
      },
    },
    MuiCard: {
      styleOverrides: {
        root: {
          borderRadius: 12,
          boxShadow: '0 1px 3px rgba(15, 23, 42, 0.04), 0 1px 2px rgba(15, 23, 42, 0.02)',
          border: '1px solid #E2E8F0',
        },
      },
    },
  },
});
