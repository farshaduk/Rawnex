'use client';
import { useState } from 'react';
import { DollarSign, Lock, ArrowUpRight, ArrowDownLeft, Clock, CheckCircle, AlertCircle, CreditCard } from 'lucide-react';

const escrows = [
  { id: 'ESC-10041', product: 'کاتد مس درجه A',  emoji: '🟡', buyer: 'فولاد یزد', seller: 'Codelco Chile', amount: '$4,225,000', funded: '$4,225,000', released: '$1,408,333', status: 'فعال',     milestone: '۱ از ۳' },
  { id: 'ESC-10038', product: 'پلت HDPE 20/60',      emoji: '⚪', buyer: 'فولاد یزد', seller: 'SABIC',        amount: '$240,000',   funded: '$240,000',   released: '$0',         status: 'تأمین‌شده', milestone: '۰ از ۲' },
  { id: 'ESC-10022', product: 'شمش آلومینیوم T6',  emoji: '🔘', buyer: 'فولاد یزد', seller: 'Norsk Hydro',  amount: '$795,000',   funded: '$795,000',   released: '$795,000',   status: 'تمام‌شده',  milestone: '۳ از ۳' },
];

const transactions = [
  { id: 'TXN-88841', type: 'debit',  desc: 'امانت تأمین‌شد — ESC-10038',       amount: '-$240,000',   date: '۱۵ دی', status: 'تکمیل‌شده' },
  { id: 'TXN-88820', type: 'credit', desc: 'مرحله آزاد شد — ESC-10041',       amount: '+$1,408,333', date: '۱۰ دی', status: 'تکمیل‌شده' },
  { id: 'TXN-88801', type: 'debit',  desc: 'امانت تأمین‌شد — ESC-10041',       amount: '-$4,225,000', date: '۲۰ آذر', status: 'تکمیل‌شده' },
  { id: 'TXN-88790', type: 'credit', desc: 'آزادسازی کامل — ESC-10022',       amount: '+$795,000',   date: '۱۵ آذر', status: 'تکمیل‌شده' },
];

const escrowStatusColor: Record<string, string> = { 'فعال': '#0A84FF', 'تأمین‌شده': '#FF9F0A', 'تمام‌شده': '#30D158' };

export default function PaymentsPage() {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
      <div>
        <h1 style={{ fontSize: '28px', fontWeight: 700, letterSpacing: '-0.03em' }}>پرداخت‌ها و امانت‌داری</h1>
        <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px' }}>مدیریت پرداخت و امانت‌داری با پشتیبان بلاک‌چین</p>
      </div>

      {/* Wallet summary */}
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr 1fr', gap: '16px' }}>
        {[
          { icon: DollarSign, label: 'موجودی قابل‌استفاده',  value: '$1,847,920', color: '#30D158', sub: 'آماده استفاده'     },
          { icon: Lock,       label: 'در امانت',          value: '$4,465,000', color: '#FF9F0A', sub: '۲ امانت فعال'   },
          { icon: ArrowUpRight,label: 'مجموع ارسال‌شده',        value: '$12.8M',    color: '#FF453A', sub: 'YTD 2025'            },
          { icon: ArrowDownLeft,label:'مجموع دریافت‌شده',    value: '$9.4M',    color: '#0A84FF', sub: 'YTD 2025'            },
        ].map((s) => {
          const Icon = s.icon;
          return (
            <div key={s.label} style={{ background: 'var(--bg-card)', borderRadius: '18px', padding: '20px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)' }}>
              <div style={{ width: '40px', height: '40px', borderRadius: '12px', background: `${s.color}18`, display: 'flex', alignItems: 'center', justifyContent: 'center', marginBottom: '12px' }}>
                <Icon size={18} style={{ color: s.color }} />
              </div>
              <p style={{ fontSize: '11px', color: 'var(--text-secondary)', fontWeight: 500, textTransform: 'uppercase', letterSpacing: '0.06em' }}>{s.label}</p>
              <p style={{ fontSize: '26px', fontWeight: 700, letterSpacing: '-0.03em', marginTop: '4px' }}>{s.value}</p>
              <p style={{ fontSize: '12px', color: 'var(--text-tertiary)', marginTop: '2px' }}>{s.sub}</p>
            </div>
          );
        })}
      </div>

      {/* Escrow accounts */}
      <div style={{ background: 'var(--bg-card)', borderRadius: '18px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', overflow: 'hidden' }}>
        <div style={{ padding: '20px 24px', borderBottom: '1px solid var(--border-subtle)' }}>
          <h2 style={{ fontSize: '16px', fontWeight: 700 }}>حساب‌های امانت‌داری فعال</h2>
        </div>
        <div style={{ display: 'flex', flexDirection: 'column', gap: '0' }}>
          {escrows.map((e) => {
            const pct = Math.round((parseFloat(e.released.replace(/[$,]/g, '')) / parseFloat(e.funded.replace(/[$,]/g, ''))) * 100) || 0;
            const sc = escrowStatusColor[e.status];
            return (
              <div key={e.id} style={{ padding: '20px 24px', borderBottom: '1px solid var(--border-subtle)' }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
                  <div style={{ width: '48px', height: '48px', borderRadius: '13px', background: 'var(--bg-input)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '22px', flexShrink: 0 }}>{e.emoji}</div>
                  <div style={{ flex: 1, minWidth: 0 }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: '8px' }}>
                      <div>
                        <p style={{ fontSize: '14px', fontWeight: 700, color: 'var(--text-primary)' }}>{e.product}</p>
                        <p style={{ fontSize: '12px', color: 'var(--text-tertiary)' }}>{e.id} · {e.buyer} ↔ {e.seller} · مرحله {e.milestone}</p>
                      </div>
                      <div style={{ textAlign: 'right' }}>
                        <p style={{ fontSize: '18px', fontWeight: 700, color: 'var(--text-primary)' }}>{e.amount}</p>
                        <span style={{ padding: '3px 10px', borderRadius: '999px', background: `${sc}20`, color: sc, fontSize: '11px', fontWeight: 600, textTransform: 'capitalize' }}>{e.status}</span>
                      </div>
                    </div>
                    {/* Progress bar */}
                    <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                      <div style={{ flex: 1, height: '6px', borderRadius: '3px', background: 'var(--bg-input)' }}>
                        <div style={{ width: `${pct}%`, height: '100%', borderRadius: '3px', background: sc, transition: 'width 500ms ease' }} />
                      </div>
                      <span style={{ fontSize: '12px', fontWeight: 600, color: 'var(--text-secondary)', flexShrink: 0 }}>{pct}% آزادشده</span>
                    </div>
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      </div>

      {/* Transaction history */}
      <div style={{ background: 'var(--bg-card)', borderRadius: '18px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', overflow: 'hidden' }}>
        <div style={{ padding: '20px 24px', borderBottom: '1px solid var(--border-subtle)' }}>
          <h2 style={{ fontSize: '16px', fontWeight: 700 }}>تاریخچه تراکنش‌ها</h2>
        </div>
        <table style={{ width: '100%', borderCollapse: 'collapse' }}>
          <thead>
            <tr style={{ background: 'var(--bg-input)' }}>
              {['شناسه تراکنش', 'توضیحات', 'مبلغ', 'تاریخ', 'وضعیت'].map(h => (
                <th key={h} style={{ padding: '11px 20px', fontSize: '11px', fontWeight: 600, color: 'var(--text-tertiary)', textTransform: 'uppercase', letterSpacing: '0.06em', textAlign: 'left' }}>{h}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {transactions.map((t) => (
              <tr key={t.id} style={{ borderTop: '1px solid var(--border-subtle)' }}>
                <td style={{ padding: '14px 20px', fontSize: '13px', color: 'var(--text-tertiary)', fontFamily: 'monospace' }}>{t.id}</td>
                <td style={{ padding: '14px 20px', fontSize: '14px', color: 'var(--text-primary)', fontWeight: 500 }}>{t.desc}</td>
                <td style={{ padding: '14px 20px', fontSize: '15px', fontWeight: 700, color: t.type === 'credit' ? '#30D158' : '#FF453A' }}>{t.amount}</td>
                <td style={{ padding: '14px 20px', fontSize: '13px', color: 'var(--text-secondary)' }}>{t.date}</td>
                <td style={{ padding: '14px 20px' }}>
                  <span style={{ padding: '3px 10px', borderRadius: '999px', background: '#30D15820', color: '#30D158', fontSize: '12px', fontWeight: 600 }}>تکمیل‌شده</span>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
