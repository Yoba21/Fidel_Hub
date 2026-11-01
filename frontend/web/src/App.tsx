import { useState } from 'react';
import { ThemeProvider, CssBaseline, Box } from '@mui/material';
import { fidelTheme } from './theme/fidelTheme';
import { Header } from './components/Header';
import { Sidebar } from './components/Sidebar';
import { Dashboard } from './components/Dashboard';

export function App() {
  const [activeTab, setActiveTab] = useState('dashboard');

  return (
    <ThemeProvider theme={fidelTheme}>
      <CssBaseline />
      <Box sx={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
        <Header tenantName="Fidel Hub Academy" />
        <Box sx={{ display: 'flex', flexGrow: 1 }}>
          <Sidebar activeTab={activeTab} onTabChange={setActiveTab} />
          <Box component="main" sx={{ flexGrow: 1, backgroundColor: '#FAF7F0', minHeight: 'calc(100vh - 70px)' }}>
            <Dashboard />
          </Box>
        </Box>
      </Box>
    </ThemeProvider>
  );
}

export default App;
