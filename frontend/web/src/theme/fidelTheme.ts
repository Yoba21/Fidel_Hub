import { createTheme } from '@mui/material/styles';

export const fidelTheme = createTheme({
  palette: {
    primary: {
      main: '#1B5E3F', // Deep Green
      light: '#2D8A5E',
      dark: '#12422C',
      contrastText: '#FFFFFF',
    },
    secondary: {
      main: '#E0A526', // Warm Gold
      light: '#EDB94C',
      dark: '#B88214',
      contrastText: '#1F2933',
    },
    text: {
      primary: '#1F2933', // Dark Ink
      secondary: '#4B5563',
    },
    background: {
      default: '#FAF7F0', // Off-White
      paper: '#FFFFFF',
    },
    divider: '#E5E7EB',
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
      color: '#1F2933',
    },
    h5: {
      fontWeight: 600,
      color: '#1F2933',
    },
    h6: {
      fontWeight: 600,
      color: '#1F2933',
    },
    subtitle1: {
      color: '#4B5563',
    },
    button: {
      textTransform: 'none',
      fontWeight: 600,
    },
  },
  shape: {
    borderRadius: 10,
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 8,
          boxShadow: 'none',
          '&:hover': {
            boxShadow: '0 2px 4px rgba(27, 94, 63, 0.2)',
          },
        },
        containedPrimary: {
          backgroundColor: '#1B5E3F',
          '&:hover': {
            backgroundColor: '#164E34',
          },
        },
      },
    },
    MuiCard: {
      styleOverrides: {
        root: {
          borderRadius: 12,
          boxShadow: '0 1px 3px rgba(0, 0, 0, 0.05), 0 1px 2px rgba(0, 0, 0, 0.03)',
          border: '1px solid #E5E7EB',
        },
      },
    },
  },
});
