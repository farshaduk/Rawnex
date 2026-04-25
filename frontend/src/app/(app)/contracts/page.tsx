'use client';
import { useState } from 'react';
import { FileText, Download, Eye, CheckCircle, Clock, AlertCircle, XCircle, Shield } from 'lucide-react';

const contracts = [
  { id: 'CNT-2025-0104', product: 'کاتد مس درجه A',  emoji: '🟡', buyer: 'فولاد یزد', seller: 'Codelco Chile', value: '$4,225,000', qty: '500 MT', start: '۱ بهمن', end: '۳۰ فروردین', payment: 'LC 90 days', status: 'فعال',       signed: '۲۰ دی' },
  { id: 'CNT-2025-0095', product: 'پلت HDPE 20/60',      emoji: '⚪', buyer: 'فولاد یزد', seller: 'SABIC',        value: '$240,000',   qty: '200 MT', start: '۱۵ دی',    end: '۱۵ فروردین',  payment: 'TT Advance',  status: 'فعال',       signed: '۱۰ دی' },
  { id: 'CNT-2025-0088', product: 'سنگ آهن ۶۲٪ Fe',      emoji: '🟤', buyer: 'فولاد یزد', seller: 'Vale S.A.',    value: '$590,000',   qty: '5000 MT',start: '۱ اسفند', end: '۳۰ اسفند',  payment: 'TT 30 days', status: 'در انتظار',    signed: '—'          },
  { id: 'CNT-2024-0512', product: 'شمش آلومینیوم T6',    emoji: '🔘', buyer: 'فولاد یزد', seller: 'Norsk Hydro',  value: '$795,000',   qty: '300 MT', start: '۱ آذر', end: '۳۱ دی',    payment: 'LC 60 days', status: 'تکمیل‌شده', signed: '۲۵ آبان' },
  { id: 'CNT-2024-0481', product: 'روی گرید LME',       emoji: '🔷', buyer: 'فولاد یزد', seller: 'Teck',         value: '$143,000',   qty: '100 MT', start: '۱ آبان', end: '۳۰ آبان',   payment: 'TT Advance', status: 'تکمیل‌شده', signed: '۲۸ مهر' },
  { id: 'CNT-2024-0450', product: 'اسید سولفوریک ۹۸٪',   emoji: '🟣', buyer: 'فولاد یزد', seller: 'Olin Corp',    value: '$9,500',     qty: '100 MT', start: '۱ مهر', end: '۳۱ مهر',    payment: 'TT Advance', status: 'لغوشده',   signed: '—'          },
];

const statusConfig: Record<string, { color: string; icon: React.ElementType; label: string }> = {
  'فعال':    { color: '#30D158', icon: CheckCircle, label: 'فعال'    },
  'در انتظار':   { color: '#FF9F0A', icon: Clock,       label: 'در انتظار'   },
  'تکمیل‌شده': { color: '#636366', icon: Shield,      label: 'تکمیل‌شده' },
  'لغوشده': { color: '#FF453A', icon: XCircle,     label: 'لغوشده' },
};

const stats = [
  { label: 'قراردادهای فعال', value: '12',  color: '#30D158', icon: CheckCircle },
  { label: 'در انتظار امضا',value: '3',   color: '#FF9F0A', icon: Clock       },
  { label: 'ارزش کل',      value: '$8.4M', color: '#0A84FF', icon: FileText  },
  { label: 'تکمیل‌شده',        value: '47',  color: '#636366', icon: Shield      },
];

export default function ContractsPage() {
  const [filter, setFilter] = useState('all');
  const filtered = filter === 'همه' ? contracts : contracts.filter(c => c.status === filter);

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
      <div>
        <h1 style={{ fontSize: '28px', fontWeight: 700, letterSpacing: '-0.03em' }}>قراردادها</h1>
        <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px' }}>قراردادهای هوشمند و توافقنامه‌های تجاری الزام‌آور</p>
      </div>

      {/* Stats */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '16px' }}>
        {stats.map((s) => {
          const Icon = s.icon;
          return (
            <div key={s.label} style={{ background: 'var(--bg-card)', borderRadius: '18px', padding: '20px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', display: 'flex', alignItems: 'center', gap: '14px' }}>
              <div style={{ width: '44px', height: '44px', borderRadius: '13px', background: `${s.color}18`, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
                <Icon size={20} style={{ color: s.color }} />
              </div>
              <div>
                <p style={{ fontSize: '11px', color: 'var(--text-secondary)', fontWeight: 500, textTransform: 'uppercase', letterSpacing: '0.06em' }}>{s.label}</p>
                <p style={{ fontSize: '24px', fontWeight: 700, letterSpacing: '-0.03em' }}>{s.value}</p>
              </div>
            </div>
          );
        })}
      </div>

      {/* Filters */}
      <div style={{ display: 'flex', gap: '8px' }}>
        {['همه', 'فعال', 'در انتظار', 'تکمیل‌شده', 'لغوشده'].map((f) => (
          <button key={f} onClick={() => setFilter(f)} style={{ padding: '7px 18px', borderRadius: '999px', border: `1.5px solid ${filter === f ? 'var(--color-brand-primary)' : 'var(--border-default)'}`, background: filter === f ? 'var(--color-brand-primary)' : 'transparent', color: filter === f ? '#fff' : 'var(--text-secondary)', fontFamily: 'inherit', fontSize: '13px', fontWeight: 600, cursor: 'pointer', transition: 'all 150ms' }}>
            {f === 'همه' ? 'همه قراردادها' : f}
          </button>
        ))}
      </div>

      {/* Contracts list */}
      <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
        {filtered.map((c) => {
          const sc = statusConfig[c.status];
          const SIcon = sc.icon;
          return (
            <div key={c.id} style={{ background: 'var(--bg-card)', borderRadius: '18px', padding: '20px 24px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', display: 'flex', alignItems: 'center', gap: '20px' }}>
              <div style={{ width: '52px', height: '52px', borderRadius: '14px', background: 'var(--bg-input)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '24px', flexShrink: 0 }}>{c.emoji}</div>
              <div style={{ flex: 1, minWidth: 0 }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', gap: '12px' }}>
                  <div>
                    <p style={{ fontSize: '15px', fontWeight: 700, color: 'var(--text-primary)' }}>{c.product}</p>
                    <p style={{ fontSize: '12px', color: 'var(--text-tertiary)', marginTop: '2px' }}>
                      {c.id} · {c.buyer} ↔ {c.seller}
                    </p>
                  </div>
                  <span style={{ padding: '4px 12px', borderRadius: '999px', background: `${sc.color}20`, color: sc.color, fontSize: '12px', fontWeight: 600, display: 'flex', alignItems: 'center', gap: '5px', flexShrink: 0 }}>
                    <SIcon size={11} />{sc.label}
                  </span>
                </div>
                <div style={{ display: 'grid', gridTemplateColumns: 'repeat(5, 1fr)', gap: '12px', marginTop: '14px' }}>
                  {[
                    { k: 'ارزش',   v: c.value   },
                    { k: 'مقدار',     v: c.qty     },
                    { k: 'دوره',  v: `${c.start} ← ${c.end}` },
                    { k: 'پرداخت', v: c.payment },
                    { k: 'امضاشده',  v: c.signed  },
                  ].map((item) => (
                    <div key={item.k}>
                      <p style={{ fontSize: '11px', color: 'var(--text-tertiary)', fontWeight: 500, textTransform: 'uppercase', letterSpacing: '0.05em' }}>{item.k}</p>
                      <p style={{ fontSize: '13px', fontWeight: 600, color: 'var(--text-primary)', marginTop: '2px' }}>{item.v}</p>
                    </div>
                  ))}
                </div>
              </div>
              <div style={{ display: 'flex', gap: '8px', flexShrink: 0 }}>
                <button style={{ padding: '8px 16px', borderRadius: '10px', background: 'var(--bg-input)', border: '1px solid var(--border-default)', fontFamily: 'inherit', fontSize: '13px', fontWeight: 600, color: 'var(--text-secondary)', cursor: 'pointer', display: 'flex', alignItems: 'center', gap: '6px' }}><Eye size={14} /> مشاهده</button>
                <button style={{ padding: '8px 16px', borderRadius: '10px', background: 'var(--bg-input)', border: '1px solid var(--border-default)', fontFamily: 'inherit', fontSize: '13px', fontWeight: 600, color: 'var(--text-secondary)', cursor: 'pointer', display: 'flex', alignItems: 'center', gap: '6px' }}><Download size={14} /> PDF</button>
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}
