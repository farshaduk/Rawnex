'use client';
import { useState } from 'react';
import { Clock, Gavel, TrendingUp, Users, Eye } from 'lucide-react';
import Link from 'next/link';

const auctions = [
  {
    id: 'AUC-0481', emoji: '🔶', product: 'کاتد مس درجه A', qty: '200 MT',
    startPrice: '$1,700,000', currentBid: '$1,847,000', bids: 23,
    endsIn: '1h 42m', status: 'live', watchers: 87,
  },
  {
    id: 'AUC-0480', emoji: '⛏️', product: 'سنگ آهن Fe 65٪ — لات 1,000 تن', qty: '1,000 MT',
    startPrice: '$95,000', currentBid: '$108,500', bids: 15,
    endsIn: '4h 20m', status: 'live', watchers: 42,
  },
  {
    id: 'AUC-0479', emoji: '⬜', product: 'دسته شمش آلومینیوم 1050A', qty: '100 MT',
    startPrice: '$220,000', currentBid: '$231,000', bids: 8,
    endsIn: '8h 05m', status: 'live', watchers: 25,
  },
  {
    id: 'AUC-0478', emoji: '🌾', product: 'اوره گرانوله ۴۶٪ — فله', qty: '800 MT',
    startPrice: '$240,000', currentBid: '$252,000', bids: 11,
    endsIn: 'پایان یافت', status: 'ended', watchers: 63,
  },
];

export default function AuctionsPage() {
  const [tab, setTab] = useState<'live' | 'upcoming' | 'ended'>('live');
  const displayed = tab === 'ended' ? auctions.filter(a => a.status === 'ended') : auctions.filter(a => a.status !== 'ended');

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-end', flexWrap: 'wrap', gap: '16px' }}>
        <div>
          <h1 style={{ fontSize: '28px', fontWeight: 700, letterSpacing: '-0.03em' }}>مزایده‌ها</h1>
          <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px' }}>مزایده رقابتی برای معاملات حجم بالا</p>
        </div>
      </div>

      {/* Tabs */}
      <div style={{ display: 'flex', gap: '4px', background: 'var(--bg-input)', borderRadius: '12px', padding: '4px', width: 'fit-content' }}>
        {(['live', 'upcoming', 'ended'] as const).map((t) => (
          <button key={t} onClick={() => setTab(t)} style={{ padding: '8px 20px', borderRadius: '10px', fontSize: '13px', fontWeight: 600, fontFamily: 'inherit', cursor: 'pointer', border: 'none', background: tab === t ? 'var(--bg-surface)' : 'transparent', color: tab === t ? 'var(--text-primary)' : 'var(--text-secondary)', boxShadow: tab === t ? 'var(--shadow-xs)' : 'none', transition: 'all 150ms ease', textTransform: 'capitalize' }}>
            {t === 'live' ? '🔴 زنده' : t === 'upcoming' ? '⏰ آینده' : '✅ پایان‌یافته'}
          </button>
        ))}
      </div>

      {/* Auction Cards */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(2, 1fr)', gap: '20px' }}>
        {displayed.map((a) => (
          <div key={a.id} style={{ background: 'var(--bg-card)', borderRadius: '22px', border: a.status === 'live' ? '1.5px solid rgba(10,132,255,0.30)' : '1px solid var(--border-subtle)', boxShadow: a.status === 'live' ? '0 4px 20px rgba(10,132,255,0.12)' : 'var(--shadow-card)', overflow: 'hidden', transition: 'transform 250ms ease, box-shadow 250ms ease' }}>
            {/* Card header */}
            <div style={{ padding: '20px 24px', borderBottom: '1px solid var(--border-subtle)', display: 'flex', justifyContent: 'space-between', alignItems: 'center', background: a.status === 'live' ? 'rgba(10,132,255,0.04)' : undefined }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                <div style={{ width: '44px', height: '44px', borderRadius: '12px', background: 'var(--bg-input)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '22px', flexShrink: 0 }}>{a.emoji}</div>
                <div>
                  <h3 style={{ fontSize: '14px', fontWeight: 600, color: 'var(--text-primary)', marginBottom: '2px' }}>{a.product}</h3>
                  <span style={{ fontSize: '12px', color: 'var(--text-tertiary)' }}>{a.id} · {a.qty}</span>
                </div>
              </div>
              {a.status === 'live' && (
                <span style={{ display: 'flex', alignItems: 'center', gap: '6px', padding: '4px 12px', background: 'rgba(255,69,58,0.10)', color: '#FF453A', borderRadius: '999px', fontSize: '11px', fontWeight: 700 }}>
                  <span style={{ width: '6px', height: '6px', borderRadius: '50%', background: '#FF453A', animation: 'pulse-ring 1.5s ease infinite' }} />
                  زنده
                </span>
              )}
            </div>

            {/* Bid info */}
            <div style={{ padding: '20px 24px', display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
              <div>
                <p style={{ fontSize: '11px', color: 'var(--text-tertiary)', fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.05em', marginBottom: '4px' }}>پیشنهاد فعلی</p>
                <p style={{ fontSize: '22px', fontWeight: 700, color: '#0A84FF', letterSpacing: '-0.03em' }}>{a.currentBid}</p>
              </div>
              <div>
                <p style={{ fontSize: '11px', color: 'var(--text-tertiary)', fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.05em', marginBottom: '4px' }}>قیمت پایه</p>
                <p style={{ fontSize: '16px', fontWeight: 600, color: 'var(--text-secondary)' }}>{a.startPrice}</p>
              </div>
              <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                <Gavel size={14} style={{ color: 'var(--text-tertiary)' }} />
                <span style={{ fontSize: '13px', color: 'var(--text-secondary)', fontWeight: 500 }}>{a.bids} پیشنهاد ثبت‌شده</span>
              </div>
              <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                <Eye size={14} style={{ color: 'var(--text-tertiary)' }} />
                <span style={{ fontSize: '13px', color: 'var(--text-secondary)', fontWeight: 500 }}>{a.watchers} دنبال‌کننده</span>
              </div>
            </div>

            {/* Footer */}
            <div style={{ padding: '16px 24px', borderTop: '1px solid var(--border-subtle)', display: 'flex', justifyContent: 'space-between', alignItems: 'center', background: 'var(--bg-input)' }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: '6px', color: a.status === 'live' ? '#FF453A' : 'var(--text-tertiary)' }}>
                <Clock size={14} />
                <span style={{ fontSize: '13px', fontWeight: 600 }}>{a.endsIn}</span>
              </div>
              {a.status === 'live' ? (
                <Link href={`/auctions/${a.id}`} style={{ padding: '10px 20px', borderRadius: '12px', background: 'var(--color-brand-primary)', color: '#fff', fontFamily: 'inherit', fontSize: '13px', fontWeight: 700, textDecoration: 'none', boxShadow: '0 4px 12px rgba(10,132,255,0.30)' }}>
                  ارسال پیشنهاد ←
                </Link>
              ) : (
                <Link href={`/auctions/${a.id}`} style={{ padding: '10px 20px', borderRadius: '12px', background: 'var(--bg-surface)', color: 'var(--text-secondary)', fontFamily: 'inherit', fontSize: '13px', fontWeight: 600, textDecoration: 'none', border: '1px solid var(--border-default)' }}>
                  مشاهده نتایج
                </Link>
              )}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
