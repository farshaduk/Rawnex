'use client';
import { useState } from 'react';
import { Plus, Search, Edit3, Trash2, Eye, TrendingUp, Package, BarChart3, CheckCircle } from 'lucide-react';

const listings = [
  { id: 'LST-001', product: 'کاتد مس درجه A',   emoji: '🟡', category: 'فلزات',       qty: '500 MT', price: '$8,450/MT', views: 342, inquiries: 18, status: 'فعال',      created: '۱۵ دی' },
  { id: 'LST-002', product: 'پلت HDPE 20/60',        emoji: '⚪', category: 'پلیمرها',     qty: '200 MT', price: '$1,200/MT', views: 218, inquiries: 11, status: 'فعال',      created: '۱۲ دی' },
  { id: 'LST-003', product: 'سنگ آهن ۶۲٪ Fe',       emoji: '🟤', category: 'فلزات',       qty: '5,000 MT', price: '$118/MT', views: 89,  inquiries: 4,  status: 'در انتظار',   created: '۱۰ دی' },
  { id: 'LST-004', product: 'اسید سولفوریک ۹۸٪',   emoji: '🟣', category: 'شیمیایی',     qty: '100 MT', price: '$95/MT',  views: 54,  inquiries: 2,  status: 'پیش‌نویس',    created: '۸ دی'  },
  { id: 'LST-005', product: 'شمش آلومینیوم T6',     emoji: '🔘', category: 'فلزات',       qty: '300 MT', price: '$2,650/MT', views: 178, inquiries: 9,  status: 'فعال',      created: '۵ دی'  },
  { id: 'LST-006', product: 'زغال حرارتی 6300 GAR',  emoji: '⚫', category: 'انرژی',       qty: '2,000 MT', price: '$78/MT', views: 135, inquiries: 7,  status: 'منقضی',     created: '۱ دی'  },
];

const statusColor: Record<string, string> = {
  'فعال': '#30D158', 'در انتظار': '#FF9F0A', 'پیش‌نویس': '#8E8E93', 'منقضی': '#FF453A',
};

const stats = [
  { icon: Package,    label: 'مجموع آگهی‌ها',   value: '24',   sub: '+3 این هفته'  },
  { icon: Eye,        label: 'مجموع بازدیدها',       value: '5,218',sub: '+12٪ نسبت به ماه گذشته' },
  { icon: TrendingUp, label: 'استعلام‌ها',         value: '143',  sub: '18 امروز' },
  { icon: CheckCircle,label: 'آگهی‌های فعال',   value: '19',   sub: '79٪ از کل' },
];

export default function ListingsPage() {
  const [search, setSearch] = useState('');
  const filtered = listings.filter(l => l.product.toLowerCase().includes(search.toLowerCase()));

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
        <div>
          <h1 style={{ fontSize: '28px', fontWeight: 700, letterSpacing: '-0.03em' }}>آگهی‌های من</h1>
          <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px' }}>آگهی‌های محصولات خود را در بازار رانکس مدیریت کنید</p>
        </div>
        <button style={{ display: 'flex', alignItems: 'center', gap: '8px', padding: '10px 20px', borderRadius: '12px', background: 'var(--color-brand-primary)', color: '#fff', border: 'none', fontFamily: 'inherit', fontSize: '14px', fontWeight: 600, cursor: 'pointer', boxShadow: '0 4px 12px rgba(10,132,255,0.30)' }}>
          <Plus size={16} /> آگهی جدید
        </button>
      </div>

      {/* Stats */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '16px' }}>
        {stats.map((s) => {
          const Icon = s.icon;
          return (
            <div key={s.label} style={{ background: 'var(--bg-card)', borderRadius: '18px', padding: '20px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                <div>
                  <p style={{ fontSize: '12px', color: 'var(--text-secondary)', fontWeight: 500, textTransform: 'uppercase', letterSpacing: '0.06em' }}>{s.label}</p>
                  <p style={{ fontSize: '28px', fontWeight: 700, letterSpacing: '-0.03em', marginTop: '6px' }}>{s.value}</p>
                  <p style={{ fontSize: '12px', color: 'var(--text-secondary)', marginTop: '4px' }}>{s.sub}</p>
                </div>
                <div style={{ width: '40px', height: '40px', borderRadius: '12px', background: 'var(--bg-input)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                  <Icon size={18} style={{ color: 'var(--color-brand-primary)' }} />
                </div>
              </div>
            </div>
          );
        })}
      </div>

      {/* Table */}
      <div style={{ background: 'var(--bg-card)', borderRadius: '18px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', overflow: 'hidden' }}>
        <div style={{ padding: '20px 24px', borderBottom: '1px solid var(--border-subtle)', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ fontSize: '16px', fontWeight: 700 }}>همه آگهی‌ها</h2>
          <div style={{ position: 'relative' }}>
            <Search size={14} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-tertiary)' }} />
            <input value={search} onChange={e => setSearch(e.target.value)} placeholder="جستجوی آگهی‌ها…" style={{ height: '36px', padding: '0 12px 0 34px', border: '1.5px solid var(--border-default)', borderRadius: '10px', fontFamily: 'inherit', fontSize: '13px', color: 'var(--text-primary)', background: 'var(--bg-input)', outline: 'none', width: '220px' }} />
          </div>
        </div>
        <table style={{ width: '100%', borderCollapse: 'collapse' }}>
          <thead>
            <tr style={{ background: 'var(--bg-input)' }}>
              {['محصول', 'دسته‌بندی', 'مقدار', 'قیمت', 'بازدید', 'استعلام', 'وضعیت', 'عملیات'].map(h => (
                <th key={h} style={{ padding: '12px 20px', fontSize: '11px', fontWeight: 600, color: 'var(--text-tertiary)', textTransform: 'uppercase', letterSpacing: '0.06em', textAlign: 'left', whiteSpace: 'nowrap' }}>{h}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {filtered.map((l, i) => (
              <tr key={l.id} style={{ borderTop: '1px solid var(--border-subtle)', transition: 'background 150ms ease' }}>
                <td style={{ padding: '16px 20px' }}>
                  <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                    <div style={{ width: '40px', height: '40px', borderRadius: '10px', background: 'var(--bg-input)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '20px', flexShrink: 0 }}>{l.emoji}</div>
                    <div>
                      <p style={{ fontSize: '14px', fontWeight: 600, color: 'var(--text-primary)' }}>{l.product}</p>
                      <p style={{ fontSize: '11px', color: 'var(--text-tertiary)' }}>{l.id} · {l.created}</p>
                    </div>
                  </div>
                </td>
                <td style={{ padding: '16px 20px' }}><span style={{ fontSize: '12px', fontWeight: 600, padding: '3px 10px', borderRadius: '999px', background: 'var(--bg-input)', color: 'var(--text-secondary)' }}>{l.category}</span></td>
                <td style={{ padding: '16px 20px', fontSize: '14px', color: 'var(--text-primary)', fontWeight: 500 }}>{l.qty}</td>
                <td style={{ padding: '16px 20px', fontSize: '14px', color: 'var(--text-primary)', fontWeight: 600 }}>{l.price}</td>
                <td style={{ padding: '16px 20px', fontSize: '14px', color: 'var(--text-secondary)' }}>{l.views.toLocaleString()}</td>
                <td style={{ padding: '16px 20px', fontSize: '14px', color: 'var(--color-brand-primary)', fontWeight: 600 }}>{l.inquiries}</td>
                <td style={{ padding: '16px 20px' }}>
                  <span style={{ fontSize: '12px', fontWeight: 600, padding: '4px 12px', borderRadius: '999px', background: `${statusColor[l.status]}20`, color: statusColor[l.status], textTransform: 'capitalize' }}>{l.status}</span>
                </td>
                <td style={{ padding: '16px 20px' }}>
                  <div style={{ display: 'flex', gap: '8px' }}>
                    <button style={{ padding: '6px', borderRadius: '8px', background: 'var(--bg-input)', border: 'none', cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--text-secondary)' }}><Edit3 size={14} /></button>
                    <button style={{ padding: '6px', borderRadius: '8px', background: 'var(--bg-input)', border: 'none', cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--color-red)' }}><Trash2 size={14} /></button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
