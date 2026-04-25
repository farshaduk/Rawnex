'use client';
import Link from 'next/link';
import { TrendingUp, TrendingDown, ArrowRight, Search, Filter } from 'lucide-react';

const orders = [
  { id: 'ORD-2847', product: 'کاتد مس',           emoji: '🔶', buyer: 'فولاد یزد',          seller: 'پارس کاپر',         qty: '50 MT', value: '$420,000', date: '۲۰ اسفند', status: 'در حمل',   statusColor: 'blue' },
  { id: 'ORD-2846', product: 'سنگ آهن Fe65٪',  emoji: '⛏️', buyer: 'فولاد مبارکه',    seller: 'چادرملو',        qty: '500 MT',value: '$85,000',  date: '۱۹ اسفند', status: 'تأییدشده',  statusColor: 'green' },
  { id: 'ORD-2845', product: 'گرانول HDPE',        emoji: '🏭', buyer: 'گروه پارس پلیمر',  seller: 'پتروشیمی تبریز',  qty: '20 MT', value: '$32,000',  date: '۱۸ اسفند', status: 'در انتظار',    statusColor: 'orange' },
  { id: 'ORD-2844', product: 'اوره ۴۶٪',           emoji: '🌾', buyer: 'کشاورزی اصفهان',        seller: 'پتروشیمی شیراز',   qty: '200 MT',value: '$110,000', date: '۱۷ اسفند', status: 'تکمیل‌شده',  statusColor: 'gray' },
  { id: 'ORD-2843', product: 'سود سوزآور',        emoji: '🧪', buyer: 'شیمیایی تهران',    seller: 'پتروشیمی بندرامام', qty: '10 MT', value: '$18,500',  date: '۱۶ اسفند', status: 'در اختلاف',   statusColor: 'red' },
  { id: 'ORD-2842', product: 'آلومینیوم 1050A',  emoji: '⬜', buyer: 'خودرو اراک',     seller: 'سالکو',             qty: '30 MT', value: '$69,300',  date: '۱۵ اسفند', status: 'تکمیل‌شده',  statusColor: 'gray' },
];

const statusColors: Record<string, string> = {
  'در حمل':  'rgba(10,132,255,0.10)',
  'تأییدشده':   'rgba(48,209,88,0.10)',
  'در انتظار':     'rgba(255,159,10,0.10)',
  'تکمیل‌شده':   'rgba(110,110,115,0.10)',
  'در اختلاف':    'rgba(255,69,58,0.10)',
};
const textColors: Record<string, string> = {
  'در حمل':  '#0A84FF',
  'تأییدشده':   '#30D158',
  'در انتظار':     '#FF9F0A',
  'تکمیل‌شده':   'var(--text-secondary)',
  'در اختلاف':    '#FF453A',
};

export default function OrdersPage() {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-end', flexWrap: 'wrap', gap: '16px' }}>
        <div>
          <h1 style={{ fontSize: '28px', fontWeight: 700, letterSpacing: '-0.03em' }}>سفارشات</h1>
          <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px' }}>تمامی سفارشات خرید و فروش خود را پیگیری و مدیریت کنید</p>
        </div>
      </div>

      {/* Stats */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '16px' }}>
        {[
          { label: 'مجموع سفارشات',    value: '147', change: '+8',    dir: 'up',   color: '#0A84FF' },
          { label: 'در حمل',      value: '23',  change: '+3',    dir: 'up',   color: '#30D158' },
          { label: 'پرداخت معلق', value: '12',  change: '-2',    dir: 'down', color: '#FF9F0A' },
          { label: 'ارزش کل',     value: '$2.4M',change: '+18%', dir: 'up',   color: '#BF5AF2' },
        ].map((s) => (
          <div key={s.label} style={{ background: 'var(--bg-card)', borderRadius: '18px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', padding: '20px 24px' }}>
            <p style={{ fontSize: '13px', color: 'var(--text-secondary)', fontWeight: 500, marginBottom: '12px' }}>{s.label}</p>
            <p style={{ fontSize: '32px', fontWeight: 700, letterSpacing: '-0.03em', color: s.color, marginBottom: '8px' }}>{s.value}</p>
            <p style={{ fontSize: '12px', display: 'flex', alignItems: 'center', gap: '4px', color: s.dir === 'up' ? '#30D158' : '#FF453A', fontWeight: 500 }}>
              {s.dir === 'up' ? <TrendingUp size={12} /> : <TrendingDown size={12} />}
              {s.change} <span style={{ color: 'var(--text-tertiary)', fontWeight: 400 }}>نسبت به ماه گذشته</span>
            </p>
          </div>
        ))}
      </div>

      {/* Table */}
      <div style={{ background: 'var(--bg-card)', borderRadius: '18px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', overflow: 'hidden' }}>
        <div style={{ padding: '16px 24px', borderBottom: '1px solid var(--border-subtle)', display: 'flex', gap: '12px', alignItems: 'center', background: 'var(--bg-input)' }}>
          <div style={{ position: 'relative', flex: 1, maxWidth: '320px' }}>
            <Search size={15} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-tertiary)' }} />
            <input placeholder="جستجوی سفارشات…" style={{ width: '100%', height: '36px', paddingLeft: '36px', background: 'var(--bg-surface)', border: '1px solid var(--border-subtle)', borderRadius: '10px', fontFamily: 'inherit', fontSize: '13px', color: 'var(--text-primary)', outline: 'none' }} />
          </div>
          <button style={{ display: 'flex', alignItems: 'center', gap: '6px', padding: '8px 14px', borderRadius: '10px', background: 'var(--bg-surface)', border: '1px solid var(--border-subtle)', fontFamily: 'inherit', fontSize: '13px', fontWeight: 500, color: 'var(--text-secondary)', cursor: 'pointer' }}>
            <Filter size={14} /> فیلتر
          </button>
        </div>

        <div style={{ display: 'grid', gridTemplateColumns: '2fr 1.5fr 1.5fr 1fr 1fr 1fr 40px', padding: '12px 24px', gap: '12px' }}>
          {['سفارش', 'خریدار', 'فروشنده', 'ارزش', 'تاریخ', 'وضعیت', ''].map((h) => (
            <span key={h} style={{ fontSize: '11px', fontWeight: 600, color: 'var(--text-tertiary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>{h}</span>
          ))}
        </div>

        {orders.map((o) => (
          <div key={o.id} style={{ display: 'grid', gridTemplateColumns: '2fr 1.5fr 1.5fr 1fr 1fr 1fr 40px', padding: '14px 24px', gap: '12px', alignItems: 'center', borderTop: '1px solid var(--border-subtle)', transition: 'background 150ms' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
              <div style={{ width: '36px', height: '36px', borderRadius: '10px', background: 'var(--bg-input)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '16px', flexShrink: 0 }}>{o.emoji}</div>
              <div>
                <div style={{ fontSize: '14px', fontWeight: 500, color: 'var(--text-primary)' }}>{o.product}</div>
                <div style={{ fontSize: '12px', color: 'var(--text-tertiary)' }}>{o.id}</div>
              </div>
            </div>
            <span style={{ fontSize: '13px', color: 'var(--text-secondary)' }}>{o.buyer}</span>
            <span style={{ fontSize: '13px', color: 'var(--text-secondary)' }}>{o.seller}</span>
            <span style={{ fontSize: '13px', fontWeight: 600, color: 'var(--text-primary)' }}>{o.value}</span>
            <span style={{ fontSize: '13px', color: 'var(--text-secondary)' }}>{o.date}</span>
            <span style={{ display: 'inline-flex', alignItems: 'center', padding: '4px 10px', borderRadius: '999px', fontSize: '11px', fontWeight: 600, background: statusColors[o.status], color: textColors[o.status], whiteSpace: 'nowrap' }}>{o.status}</span>
            <Link href={`/orders/${o.id}`} style={{ color: 'var(--text-brand)', textDecoration: 'none', fontSize: '14px' }}>→</Link>
          </div>
        ))}
      </div>
    </div>
  );
}
