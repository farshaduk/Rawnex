'use client';
import { Bell, Search, Sun, Moon, Menu, PanelLeftClose, PanelLeftOpen, Zap, ChevronRight } from 'lucide-react';
import styles from './Topbar.module.scss';

interface TopbarProps {
  collapsed?: boolean;
  onToggleSidebar?: () => void;
  onMobileOpen?: () => void;
  darkMode?: boolean;
  onToggleTheme?: () => void;
  pageTitle?: string;
  breadcrumbs?: { label: string; href?: string }[];
}

export default function Topbar({
  collapsed, onToggleSidebar, onMobileOpen,
  darkMode, onToggleTheme, pageTitle, breadcrumbs
}: TopbarProps) {
  return (
    <header className={[styles.topbar, collapsed ? styles['topbar--collapsed'] : ''].join(' ')}>
      <div className={styles.topbar__left}>
        {/* Mobile hamburger */}
        <button className={styles['topbar__menu-btn']} onClick={onMobileOpen} aria-label="Open menu">
          <Menu size={20} />
        </button>

        {/* Collapse toggle */}
        <button className={styles['topbar__collapse-btn']} onClick={onToggleSidebar} aria-label="Toggle sidebar">
          {collapsed ? <PanelLeftOpen size={18} /> : <PanelLeftClose size={18} />}
        </button>

        {/* Breadcrumb */}
        {breadcrumbs && breadcrumbs.length > 0 && (
          <nav className={styles.topbar__breadcrumb} aria-label="Breadcrumb">
            {breadcrumbs.map((crumb, i) => (
              <span key={i} className={styles['topbar__breadcrumb-item-wrap']} style={{ display: 'flex', alignItems: 'center', gap: '4px' }}>
                {i > 0 && <ChevronRight size={12} className={styles['topbar__breadcrumb-sep']} />}
                <span className={[
                  styles['topbar__breadcrumb-item'],
                  i === breadcrumbs.length - 1 ? styles['topbar__breadcrumb-item--active'] : ''
                ].join(' ')}>
                  {crumb.label}
                </span>
              </span>
            ))}
          </nav>
        )}

        {/* Search */}
        <div className={styles.topbar__search}>
          <span className={styles['topbar__search-icon']}>
            <Search size={15} />
          </span>
          <input
            type="text"
            placeholder="جستجو در محصولات، سفارشات، شرکت‌ها…"
            className={styles['topbar__search-input']}
            aria-label="جستجوی سراسری"
          />
          <div className={styles['topbar__search-shortcut']}>
            <span className={styles['topbar__search-key']}>⌘</span>
            <span className={styles['topbar__search-key']}>K</span>
          </div>
        </div>
      </div>

      {/* Actions */}
      <div className={styles.topbar__actions}>
        {/* AI Feature */}
        <button className={styles['topbar__action-btn']} aria-label="AI Assistant">
          <Zap size={18} />
        </button>

        {/* Notifications */}
        <button className={styles['topbar__action-btn']} aria-label="Notifications">
          <Bell size={18} />
          <span className={`${styles['topbar__badge']} ${styles['topbar__badge--count']}`}>12</span>
        </button>

        {/* Theme */}
        <button className={styles['topbar__theme-btn']} onClick={onToggleTheme} aria-label="Toggle theme">
          {darkMode ? <Sun size={18} /> : <Moon size={18} />}
        </button>

        {/* User */}
        <div className={styles.topbar__user}>
          <div className={styles['topbar__user-avatar']}>ع.ک</div>
          <span className={styles['topbar__user-name']}>علی کریمی</span>
        </div>
      </div>
    </header>
  );
}
