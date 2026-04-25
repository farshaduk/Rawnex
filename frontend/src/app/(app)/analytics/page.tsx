'use client';
import { TrendingUp, TrendingDown, BarChart2, ArrowUp, ArrowDown } from 'lucide-react';

const commodities = [
  { name: 'مس',         unit: 'USD/MT',  price: 9248,  prev: 9120,  high: 9410, low: 9080, emoji: '🔶', change: +1.4 },
  { name: 'سنگ آهن',  unit: 'USD/DMT', price: 112,   prev: 113,   high: 115,  low: 110,  emoji: '⛏️', change: -0.9 },
  { name: 'آلومینیوم', unit: 'USD/MT',  price: 2310,  prev: 2292,  high: 2340, low: 2280, emoji: '⬜', change: +0.8 },
  { name: 'روی',        unit: 'USD/MT',  price: 2680,  prev: 2720,  high: 2750, low: 2660, emoji: '🟡', change: -1.5 },
  { name: 'اوره',      unit: 'USD/MT',  price: 315,   prev: 308,   high: 320,  low: 305,  emoji: '🌾', change: +2.3 },
  { name: 'HDPE',      unit: 'USD/MT',  price: 1180,  prev: 1195,  high: 1210, low: 1170, emoji: '🏭', change: -1.3 },
];

export default function AnalyticsPage() {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
      <div>
        <h1 style={{ fontSize: '28px', fontWeight: 700, letterSpacing: '-0.03em' }}>تحلیل بازار و هوش تجاری</h1>
        <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px' }}>قیمت‌های زنده نهاده، حجم معاملات و تحلیل‌های هوش مصنوعی</p>
      </div>

      {/* Volume stats */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '16px' }}>
        {[
          { label: 'حجم ماهانه',    value: '$48.2M',  change: '+12.4%', dir: 'up' },
          { label: 'معاملات تکمیل‌شده',  value: '1,284',   change: '+8.1%',  dir: 'up' },
          { label: 'خریداران فعال',     value: '342',     change: '+15%',   dir: 'up' },
          { label: 'میانگین سفارش',    value: '$37.5K',  change: '-2.3%',  dir: 'down' },
        ].map((s) => (
          <div key={s.label} style={{ background: 'var(--bg-card)', borderRadius: '18px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', padding: '20px 24px' }}>
            <p style={{ fontSize: '13px', color: 'var(--text-secondary)', fontWeight: 500, marginBottom: '12px' }}>{s.label}</p>
            <p style={{ fontSize: '32px', fontWeight: 700, letterSpacing: '-0.03em', color: 'var(--text-primary)', marginBottom: '8px' }}>{s.value}</p>
            <p style={{ fontSize: '12px', display: 'flex', alignItems: 'center', gap: '4px', fontWeight: 500, color: s.dir === 'up' ? '#30D158' : '#FF453A' }}>
              {s.dir === 'up' ? <TrendingUp size={12} /> : <TrendingDown size={12} />}
              {s.change} <span style={{ color: 'var(--text-tertiary)', fontWeight: 400 }}>نسبت به ماه گذشته</span>
            </p>
          </div>
        ))}
      </div>

      {/* Chart placeholder */}
      <div style={{ background: 'var(--bg-card)', borderRadius: '18px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', overflow: 'hidden' }}>
        <div style={{ padding: '20px 24px', borderBottom: '1px solid var(--border-subtle)', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div>
            <span style={{ fontSize: '17px', fontWeight: 600 }}>روند حجم معاملات</span>
            <p style={{ fontSize: '12px', color: 'var(--text-secondary)', marginTop: '2px' }}>۹۰ روز گذشته</p>
          </div>
          <div style={{ display: 'flex', gap: '6px' }}>
            {['7D', '30D', '90D', '1Y'].map((t) => (
              <button key={t} style={{ padding: '6px 12px', borderRadius: '8px', fontSize: '12px', fontWeight: 600, fontFamily: 'inherit', cursor: 'pointer', background: t === '90D' ? 'var(--color-brand-primary)' : 'var(--bg-input)', color: t === '90D' ? '#fff' : 'var(--text-secondary)', border: 'none' }}>{t}</button>
            ))}
          </div>
        </div>

        <div style={{ padding: '24px', height: '280px', position: 'relative', overflow: 'hidden' }}>
          {/* SVG bar chart */}
          <svg width="100%" height="100%" viewBox="0 0 800 200" preserveAspectRatio="none" style={{ opacity: 0.9 }}>
            <defs>
              <linearGradient id="barGrad" x1="0" y1="0" x2="0" y2="1">
                <stop offset="0%" stopColor="#0A84FF" stopOpacity="1" />
                <stop offset="100%" stopColor="#0A84FF" stopOpacity="0.4" />
              </linearGradient>
              <linearGradient id="lineGrad" x1="0" y1="0" x2="1" y2="0">
                <stop offset="0%" stopColor="#0A84FF" />
                <stop offset="100%" stopColor="#BF5AF2" />
              </linearGradient>
            </defs>
            {/* Grid lines */}
            {[0.25, 0.5, 0.75, 1].map((y, i) => (
              <line key={i} x1="0" y1={y * 200} x2="800" y2={y * 200} stroke="var(--border-subtle)" strokeWidth="1" />
            ))}
            {/* Bars */}
            {[65, 78, 82, 70, 90, 95, 88, 102, 115, 98, 120, 130, 125, 136, 142, 128, 148, 155, 140, 162, 158, 145, 168, 172, 166, 178, 185, 175, 192, 188].map((h, i) => (
              <rect key={i} x={i * 27 + 2} y={200 - h} width={22} height={h} fill="url(#barGrad)" rx="4" />
            ))}
          </svg>
        </div>
      </div>

      {/* Commodity Price Table */}
      <div style={{ background: 'var(--bg-card)', borderRadius: '18px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', overflow: 'hidden' }}>
        <div style={{ padding: '20px 24px', borderBottom: '1px solid var(--border-subtle)' }}>
          <span style={{ fontSize: '17px', fontWeight: 600 }}>قیمت‌های زنده کالاها</span>
        </div>
        <div style={{ display: 'grid', gridTemplateColumns: '2fr 1fr 1fr 1fr 1fr 1fr', padding: '12px 24px', background: 'var(--bg-input)', gap: '12px' }}>
          {['کالا', 'قیمت', 'تغییر', 'بالاترین ۲۴س', 'پایین‌ترین ۲۴س', 'واحد'].map((h) => (
            <span key={h} style={{ fontSize: '11px', fontWeight: 600, color: 'var(--text-tertiary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>{h}</span>
          ))}
        </div>
        {commodities.map((c) => (
          <div key={c.name} style={{ display: 'grid', gridTemplateColumns: '2fr 1fr 1fr 1fr 1fr 1fr', padding: '14px 24px', gap: '12px', alignItems: 'center', borderTop: '1px solid var(--border-subtle)' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
              <span style={{ fontSize: '22px' }}>{c.emoji}</span>
              <span style={{ fontSize: '14px', fontWeight: 600, color: 'var(--text-primary)' }}>{c.name}</span>
            </div>
            <span style={{ fontSize: '15px', fontWeight: 700, color: 'var(--text-primary)' }}>${c.price.toLocaleString()}</span>
            <span style={{ fontSize: '13px', fontWeight: 600, display: 'flex', alignItems: 'center', gap: '3px', color: c.change > 0 ? '#30D158' : '#FF453A' }}>
              {c.change > 0 ? <ArrowUp size={12} /> : <ArrowDown size={12} />}
              {Math.abs(c.change)}%
            </span>
            <span style={{ fontSize: '13px', color: 'var(--text-secondary)' }}>${c.high.toLocaleString()}</span>
            <span style={{ fontSize: '13px', color: 'var(--text-secondary)' }}>${c.low.toLocaleString()}</span>
            <span style={{ fontSize: '12px', color: 'var(--text-tertiary)' }}>{c.unit}</span>
          </div>
        ))}
      </div>
    </div>
  );
}
