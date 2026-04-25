'use client';
import Link from 'next/link';
import { Plus, Clock, CheckCircle, AlertCircle, XCircle, ArrowRight, Filter } from 'lucide-react';
import styles from './page.module.scss';

const rfqs = [
  { id: 'RFQ-1042', product: 'کاتد مس درجه A',        qty: '100 MT', deadline: '2026-05-10', bids: 14, status: 'فعال',           statusColor: 'green',  emoji: '🔶', budget: '$925,000' },
  { id: 'RFQ-1041', product: 'اوره گرانوله ۴۶٪',      qty: '500 MT', deadline: '2026-05-05', bids: 7,  status: 'در انتظار',      statusColor: 'orange', emoji: '🌾', budget: '$157,500' },
  { id: 'RFQ-1040', product: 'پرک سود سوزآور ۹۸٪',   qty: '50 MT',  deadline: '2026-04-28', bids: 11, status: 'در انتظار',      statusColor: 'orange', emoji: '🧪', budget: '$19,250' },
  { id: 'RFQ-1039', product: 'HDPE فیلم گرید BL3',    qty: '25 MT',  deadline: '2026-04-20', bids: 9,  status: 'تخصیص‌یافته',  statusColor: 'blue',   emoji: '🏭', budget: '$29,500' },
  { id: 'RFQ-1038', product: 'شمش روی SHG',            qty: '20 MT',  deadline: '2026-04-15', bids: 5,  status: 'بسته‌شده',       statusColor: 'gray',   emoji: '🟡', budget: '$53,600' },
];

export default function RFQPage() {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-end', flexWrap: 'wrap', gap: '16px' }}>
        <div>
          <h1 style={{ fontSize: '28px', fontWeight: 700, letterSpacing: '-0.03em' }}>درخواست استعلام قیمت</h1>
          <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px' }}>درخواست‌های تدارکاتی خود را مدیریت کنید و پیشنهادات رقابتی دریافت کنید</p>
        </div>
        <Link href="/rfq/new" style={{ display: 'inline-flex', alignItems: 'center', gap: '6px', padding: '10px 20px', borderRadius: '12px', fontFamily: 'inherit', fontSize: '13px', fontWeight: 600, color: '#fff', background: 'var(--color-brand-primary)', textDecoration: 'none', boxShadow: '0 4px 14px rgba(10,132,255,0.30)' }}>
          <Plus size={15} /> درخواست جدید
        </Link>
      </div>

      {/* Stats */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '16px' }}>
        {[
          { label: 'درخواست‌های فعال',    value: '12', icon: '📄', color: '#0A84FF' },
          { label: 'مجموع پیشنهادات',     value: '87', icon: '🎯', color: '#30D158' },
          { label: 'میانگین پاسخ',   value: '4.2h',icon: '⏱️', color: '#FF9F0A' },
          { label: 'تخصیص این ماه',value: '8',  icon: '✅', color: '#BF5AF2' },
        ].map((s) => (
          <div key={s.label} style={{ background: 'var(--bg-card)', borderRadius: '18px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', padding: '20px 24px' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '12px' }}>
              <span style={{ fontSize: '13px', color: 'var(--text-secondary)', fontWeight: 500 }}>{s.label}</span>
              <span style={{ fontSize: '20px' }}>{s.icon}</span>
            </div>
            <div style={{ fontSize: '32px', fontWeight: 700, letterSpacing: '-0.03em', color: s.color }}>{s.value}</div>
          </div>
        ))}
      </div>

      {/* Table */}
      <div style={{ background: 'var(--bg-card)', borderRadius: '18px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', overflow: 'hidden' }}>
        <div style={{ padding: '20px 24px', borderBottom: '1px solid var(--border-subtle)', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <span style={{ fontSize: '17px', fontWeight: 600 }}>درخواست‌های من</span>
          <button style={{ display: 'flex', alignItems: 'center', gap: '6px', padding: '8px 16px', borderRadius: '10px', background: 'var(--bg-input)', border: '1px solid var(--border-subtle)', fontFamily: 'inherit', fontSize: '13px', fontWeight: 500, color: 'var(--text-secondary)', cursor: 'pointer' }}>
            <Filter size={14} /> فیلتر
          </button>
        </div>

        {/* Header row */}
        <div style={{ display: 'grid', gridTemplateColumns: '2fr 1fr 1fr 1fr 1fr 1fr 40px', padding: '12px 24px', background: 'var(--bg-input)', gap: '12px' }}>
          {['محصول', 'مقدار', 'بودجه', 'مهلت', 'پیشنهادات', 'وضعیت', ''].map((h) => (
            <span key={h} style={{ fontSize: '11px', fontWeight: 600, color: 'var(--text-tertiary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>{h}</span>
          ))}
        </div>

        {rfqs.map((rfq) => (
          <div key={rfq.id} style={{ display: 'grid', gridTemplateColumns: '2fr 1fr 1fr 1fr 1fr 1fr 40px', padding: '16px 24px', gap: '12px', alignItems: 'center', borderTop: '1px solid var(--border-subtle)' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
              <div style={{ width: '36px', height: '36px', borderRadius: '10px', background: 'var(--bg-input)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '16px', flexShrink: 0 }}>{rfq.emoji}</div>
              <div>
                <div style={{ fontSize: '14px', fontWeight: 500, color: 'var(--text-primary)' }}>{rfq.product}</div>
                <div style={{ fontSize: '12px', color: 'var(--text-tertiary)' }}>{rfq.id}</div>
              </div>
            </div>
            <span style={{ fontSize: '13px', color: 'var(--text-secondary)' }}>{rfq.qty}</span>
            <span style={{ fontSize: '13px', fontWeight: 600, color: 'var(--text-primary)' }}>{rfq.budget}</span>
            <span style={{ fontSize: '13px', color: 'var(--text-secondary)' }}>{rfq.deadline}</span>
            <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
              <span style={{ fontSize: '14px', fontWeight: 700, color: 'var(--text-primary)' }}>{rfq.bids}</span>
              <span style={{ fontSize: '11px', color: 'var(--text-tertiary)' }}>پیشنهاد</span>
            </div>
            <span style={{ display: 'inline-flex', alignItems: 'center', padding: '4px 10px', borderRadius: '999px', fontSize: '11px', fontWeight: 600, background: rfq.statusColor === 'green' ? 'rgba(48,209,88,0.10)' : rfq.statusColor === 'orange' ? 'rgba(255,159,10,0.10)' : rfq.statusColor === 'blue' ? 'rgba(10,132,255,0.10)' : 'rgba(110,110,115,0.10)', color: rfq.statusColor === 'green' ? '#30D158' : rfq.statusColor === 'orange' ? '#FF9F0A' : rfq.statusColor === 'blue' ? '#0A84FF' : 'var(--text-secondary)' }}>{rfq.status}</span>
            <Link href={`/rfq/${rfq.id}`} style={{ color: 'var(--text-brand)', textDecoration: 'none', fontSize: '14px' }}>→</Link>
          </div>
        ))}
      </div>
    </div>
  );
}
