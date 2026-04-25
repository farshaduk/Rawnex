'use client';
import { useState } from 'react';
import { Bell, MessageCircle, FileText, DollarSign, AlertCircle, CheckCircle, Truck, Gavel, X } from 'lucide-react';

const notifications = [
  { id: 1,  type: 'bid',      icon: Gavel,         color: '#0A84FF', title: 'پیشنهاد جدید برای AUC-2025-0041',       body: 'شرکت Baosteel پیشنهادی به مبلغ $8,320/MT برای مزایده کاتد مس شما ثبت کرد.',            time: '۲ دقیقه پیش',  unread: true  },
  { id: 2,  type: 'message',  icon: MessageCircle, color: '#BF5AF2', title: 'پیام از SABIC',              body: 'اسناد به مذاکره NEG-2025-0038 پیوست شدند. لطفاً بررسی و امضا کنید.',         time: '۱۵ دقیقه پیش', unread: true  },
  { id: 3,  type: 'payment',  icon: DollarSign,    color: '#30D158', title: 'مرحله امانت‌داری آزادشد',       body: 'مرحله ۱ از ۳ ($1,408,333) برای ESC-10041 — کاتد مس آزادشد.',           time: '۱ ساعت پیش',     unread: true  },
  { id: 4,  type: 'rfq',      icon: FileText,      color: '#FF9F0A', title: 'پیشنهاد جدید برای RFQ-2025-0312',      body: 'Vale S.A. پیشنهاد رقابتی، قیمت $115/MT CIF ارائه داد. قبل از انقضا بررسی کنید.',      time: '۲ ساعت پیش',     unread: false },
  { id: 5,  type: 'shipment', icon: Truck,         color: '#FF9F0A', title: 'محموله SHP-2025-0198 در گمرک', body: 'بار HDPE شما به گمرک بندرعباس رسید. ممکن است اقدام لازم باشد.',time: '۴ ساعت پیش',     unread: false },
  { id: 6,  type: 'alert',    icon: AlertCircle,   color: '#FF453A', title: 'هشدار قیمت: مس ‎+2.3٪',       body: 'قیمت لحظه‌ای کاتد مس از آستانه هشدار $8,400/MT شما بالاتر رفت.',           time: '۶ ساعت پیش',     unread: false },
  { id: 7,  type: 'contract', icon: FileText,      color: '#30D158', title: 'قرارداد CNT-2025-0088 در انتظار', body: 'قرارداد سنگ آهن منتظر امضای شماست. مهلت: ۵ بهمن.',                  time: '۱ روز پیش',     unread: false },
  { id: 8,  type: 'message',  icon: MessageCircle, color: '#BF5AF2', title: 'پیشنهاد متقابل از Codelco',      body: 'Codelco به مذاکره شما پاسخ داد: $8,380/MT، تعهد ۳ ماهه.',             time: '۱ روز پیش',     unread: false },
  { id: 9,  type: 'system',   icon: CheckCircle,   color: '#636366', title: 'تأیید هویت شرکت انجام شد',  body: 'فولاد یزد توسط تیم اطمینان رانکس تأیید شد.',            time: '۳ روز پیش',     unread: false },
  { id: 10, type: 'bid',      icon: Gavel,         color: '#0A84FF', title: 'پیشنهاد بازنده در AUC-2025-0039',        body: 'پیشنهاد بالاتری به مبلغ $118.50/MT ثبت شد. پیشنهاد شما دیگر برنده نیست.',        time: '۳ روز پیش',     unread: false },
];

type Filter = 'all' | 'unread' | 'bids' | 'messages' | 'payments';

const filterTabs: { key: Filter; label: string }[] = [
  { key: 'all',      label: 'همه'           },
  { key: 'unread',   label: 'خوانده‌نشده'   },
  { key: 'bids',     label: 'پیشنهادات'     },
  { key: 'messages', label: 'پیام‌ها' },
  { key: 'payments', label: 'پرداخت‌ها' },
];

export default function NotificationsPage() {
  const [filter, setFilter] = useState<Filter>('all');
  const [dismissed, setDismissed] = useState<number[]>([]);

  const visible = notifications.filter(n => !dismissed.includes(n.id)).filter(n => {
    if (filter === 'all')      return true;
    if (filter === 'unread')   return n.unread;
    if (filter === 'bids')     return n.type === 'bid';
    if (filter === 'messages') return n.type === 'message';
    if (filter === 'payments') return n.type === 'payment';
    return true;
  });

  const unreadCount = notifications.filter(n => n.unread && !dismissed.includes(n.id)).length;

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
          <div>
            <h1 style={{ fontSize: '28px', fontWeight: 700, letterSpacing: '-0.03em' }}>اعلانات</h1>
            <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px' }}>از معاملات، پیشنهادات و پیام‌هایتان آگاه بمانید</p>
          </div>
          {unreadCount > 0 && (
            <span style={{ padding: '3px 12px', borderRadius: '999px', background: 'var(--color-brand-primary)', color: '#fff', fontSize: '13px', fontWeight: 700 }}>{unreadCount}</span>
          )}
        </div>
        <button style={{ padding: '9px 18px', borderRadius: '12px', background: 'var(--bg-card)', border: '1px solid var(--border-default)', fontFamily: 'inherit', fontSize: '13px', fontWeight: 600, color: 'var(--text-secondary)', cursor: 'pointer' }}>
          خواندن همه
        </button>
      </div>

      {/* Filter tabs */}
      <div style={{ display: 'flex', gap: '8px' }}>
        {filterTabs.map(t => (
          <button key={t.key} onClick={() => setFilter(t.key)} style={{ padding: '7px 18px', borderRadius: '999px', border: `1.5px solid ${filter === t.key ? 'var(--color-brand-primary)' : 'var(--border-default)'}`, background: filter === t.key ? 'var(--color-brand-primary)' : 'transparent', color: filter === t.key ? '#fff' : 'var(--text-secondary)', fontFamily: 'inherit', fontSize: '13px', fontWeight: 600, cursor: 'pointer', transition: 'all 150ms' }}>
            {t.label}
          </button>
        ))}
      </div>

      {/* Notification list */}
      <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
        {visible.length === 0 && (
          <div style={{ padding: '48px', display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '12px', color: 'var(--text-tertiary)', background: 'var(--bg-card)', borderRadius: '18px', border: '1px solid var(--border-subtle)' }}>
            <Bell size={40} style={{ opacity: 0.3 }} />
            <p style={{ fontSize: '15px', fontWeight: 500 }}>اعلانی وجود ندارد</p>
          </div>
        )}
        {visible.map((n) => {
          const Icon = n.icon;
          return (
            <div key={n.id} style={{ background: 'var(--bg-card)', borderRadius: '16px', padding: '16px 20px', border: `1px solid ${n.unread ? 'rgba(10,132,255,0.25)' : 'var(--border-subtle)'}`, boxShadow: n.unread ? '0 0 0 3px rgba(10,132,255,0.06)' : 'var(--shadow-card)', display: 'flex', alignItems: 'flex-start', gap: '14px', position: 'relative', transition: 'all 200ms ease' }}>
              {n.unread && <span style={{ position: 'absolute', top: '18px', left: '8px', width: '6px', height: '6px', borderRadius: '50%', background: 'var(--color-brand-primary)' }} />}
              <div style={{ width: '42px', height: '42px', borderRadius: '13px', background: `${n.color}18`, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
                <Icon size={18} style={{ color: n.color }} />
              </div>
              <div style={{ flex: 1, minWidth: 0 }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', gap: '12px' }}>
                  <p style={{ fontSize: '14px', fontWeight: n.unread ? 700 : 600, color: 'var(--text-primary)' }}>{n.title}</p>
                  <div style={{ display: 'flex', alignItems: 'center', gap: '8px', flexShrink: 0 }}>
                    <span style={{ fontSize: '12px', color: 'var(--text-tertiary)' }}>{n.time}</span>
                    <button onClick={() => setDismissed(d => [...d, n.id])} style={{ padding: '3px', borderRadius: '6px', background: 'transparent', border: 'none', cursor: 'pointer', color: 'var(--text-tertiary)', display: 'flex' }}><X size={13} /></button>
                  </div>
                </div>
                <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px', lineHeight: 1.5 }}>{n.body}</p>
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}
