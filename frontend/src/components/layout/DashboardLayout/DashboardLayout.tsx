'use client';
import { useState, useEffect } from 'react';
import Sidebar from '../Sidebar/Sidebar';
import Topbar from '../Topbar/Topbar';
import styles from './DashboardLayout.module.scss';

interface DashboardLayoutProps {
  children: React.ReactNode;
  breadcrumbs?: { label: string; href?: string }[];
}

export default function DashboardLayout({ children, breadcrumbs }: DashboardLayoutProps) {
  const [collapsed, setCollapsed] = useState(false);
  const [mobileOpen, setMobileOpen] = useState(false);
  const [darkMode, setDarkMode] = useState(false);

  useEffect(() => {
    const stored = localStorage.getItem('theme');
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    if (stored === 'dark' || (!stored && prefersDark)) {
      setDarkMode(true);
      document.documentElement.setAttribute('data-theme', 'dark');
    }
  }, []);

  const toggleTheme = () => {
    const next = !darkMode;
    setDarkMode(next);
    document.documentElement.setAttribute('data-theme', next ? 'dark' : 'light');
    localStorage.setItem('theme', next ? 'dark' : 'light');
  };

  return (
    <div className={styles.layout}>
      <Sidebar
        collapsed={collapsed}
        mobileOpen={mobileOpen}
        onMobileClose={() => setMobileOpen(false)}
        onToggle={() => setCollapsed(c => !c)}
      />

      <main className={[styles.layout__main, collapsed ? styles['layout__main--collapsed'] : ''].join(' ')}>
        <Topbar
          collapsed={collapsed}
          onToggleSidebar={() => setCollapsed(c => !c)}
          onMobileOpen={() => setMobileOpen(true)}
          darkMode={darkMode}
          onToggleTheme={toggleTheme}
          breadcrumbs={breadcrumbs}
        />
        <div className={styles.layout__content}>
          <div className={styles.layout__page}>
            {children}
          </div>
        </div>
      </main>
    </div>
  );
}
