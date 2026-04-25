'use client';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import {
  LayoutDashboard, Package, ShoppingCart, FileText,
  Gavel, Truck, BarChart2, MessageSquare, Bell,
  Settings, Shield, Users, Building2, CreditCard,
  ClipboardList, Star, ChevronRight, X, Zap
} from 'lucide-react';
import styles from './Sidebar.module.scss';

interface SidebarProps {
  collapsed?: boolean;
  mobileOpen?: boolean;
  onMobileClose?: () => void;
  onToggle?: () => void;
}

const navSections = [
  {
    title: 'نمای کلی',
    items: [
      { label: 'داشبورد', icon: LayoutDashboard, href: '/dashboard', badge: null },
      { label: 'آمار و تحلیل', icon: BarChart2, href: '/analytics', badge: null },
    ],
  },
  {
    title: 'معاملات',
    items: [
      { label: 'بازار محصولات', icon: Package, href: '/marketplace', badge: null },
      { label: 'آگهی‌های من', icon: ClipboardList, href: '/listings', badge: null },
      { label: 'درخواست قیمت', icon: FileText, href: '/rfq', badge: '۳', badgeColor: 'blue' },
      { label: 'مذاکرات', icon: MessageSquare, href: '/negotiations', badge: '۲', badgeColor: 'orange' },
      { label: 'مزایده‌ها', icon: Gavel, href: '/auctions', badge: null },
    ],
  },
  {
    title: 'عملیات',
    items: [
      { label: 'سفارشات', icon: ShoppingCart, href: '/orders', badge: '۵', badgeColor: 'red' },
      { label: 'قراردادها', icon: FileText, href: '/contracts', badge: null },
      { label: 'پرداخت‌ها', icon: CreditCard, href: '/payments', badge: null },
      { label: 'لجستیک', icon: Truck, href: '/logistics', badge: null },
      { label: 'کنترل کیفیت', icon: Star, href: '/quality', badge: null },
    ],
  },
  {
    title: 'شرکت',
    items: [
      { label: 'شرکت‌ها', icon: Building2, href: '/companies', badge: null },
      { label: 'تیم', icon: Users, href: '/team', badge: null },
      { label: 'اعلانات', icon: Bell, href: '/notifications', badge: '۱۲', badgeColor: 'blue' },
    ],
  },
  {
    title: 'مدیریت',
    items: [
      { label: 'پنل مدیریت', icon: Shield, href: '/admin', badge: null },
      { label: 'تنظیمات', icon: Settings, href: '/settings', badge: null },
    ],
  },
];

export default function Sidebar({ collapsed, mobileOpen, onMobileClose, onToggle }: SidebarProps) {
  const pathname = usePathname();

  const sidebarClass = [
    styles.sidebar,
    collapsed ? styles['sidebar--collapsed'] : '',
    mobileOpen ? styles['sidebar--open'] : '',
  ].filter(Boolean).join(' ');

  return (
    <>
      {mobileOpen && (
        <div className={styles['sidebar-overlay']} onClick={onMobileClose} />
      )}
      <aside className={sidebarClass}>
        {/* Logo */}
        <Link href="/dashboard" className={styles.sidebar__logo}>
          <div className={styles['sidebar__logo-icon']}>R</div>
          <span className={styles['sidebar__logo-text']}>
            Raw<span>nex</span>
          </span>
        </Link>

        {/* Navigation */}
        <nav className={styles.sidebar__nav}>
          {navSections.map((section) => (
            <div key={section.title} className={styles.sidebar__section}>
              <p className={styles['sidebar__section-title']}>{section.title}</p>
              {section.items.map((item) => {
                const Icon = item.icon;
                const isActive = pathname === item.href || pathname.startsWith(item.href + '/');
                return (
                  <Link
                    key={item.href}
                    href={item.href}
                    className={[
                      styles.sidebar__item,
                      isActive ? styles['sidebar__item--active'] : '',
                    ].filter(Boolean).join(' ')}
                    title={collapsed ? item.label : undefined}
                  >
                    <span className={styles['sidebar__item-icon']}>
                      <Icon size={18} strokeWidth={1.8} />
                    </span>
                    <span className={styles['sidebar__item-label']}>{item.label}</span>
                    {item.badge && !collapsed && (
                      <span className={[
                        styles['sidebar__item-badge'],
                        item.badgeColor === 'red' ? styles['sidebar__item-badge--red'] : '',
                        item.badgeColor === 'orange' ? styles['sidebar__item-badge--orange'] : '',
                      ].filter(Boolean).join(' ')}>
                        {item.badge}
                      </span>
                    )}
                  </Link>
                );
              })}
            </div>
          ))}
        </nav>

        {/* User Profile */}
        <div className={styles.sidebar__user}>
          <div className={styles['sidebar__user-avatar']}>ع.ک</div>
          <div className={styles['sidebar__user-info']}>
            <p className={styles['sidebar__user-name']}>علی کریمی</p>
            <p className={styles['sidebar__user-role']}>مدیر · تأییدشده</p>
          </div>
        </div>
      </aside>
    </>
  );
}
