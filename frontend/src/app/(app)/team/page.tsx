'use client';
import { useState } from 'react';
import { UserPlus, Mail, MoreHorizontal, Shield, User, Crown, Search, CheckCircle } from 'lucide-react';

const members = [
  { name: 'علی کریمی',        email: 'ali.karimi@yazd-steel.com',       role: 'مالک',       dept: 'مدیریت',    status: 'active', last: 'همین‌الان',     initials: 'ع.ک', color: '#0A84FF' },
  { name: 'سارا حسینی',     email: 'sara.h@yazd-steel.com',           role: 'مدیر',      dept: 'تدارکات',   status: 'active', last: '۵ دقیقه پیش',   initials: 'س.ح', color: '#BF5AF2' },
  { name: 'رضا محمدی',    email: 'r.mohammadi@yazd-steel.com',      role: 'معامله‌گر',  dept: 'معاملات',     status: 'active', last: '۱ ساعت پیش',    initials: 'ر.م', color: '#30D158' },
  { name: 'نسرین علی‌زاده',   email: 'n.alizadeh@yazd-steel.com',       role: 'خریدار',      dept: 'تدارکات',   status: 'active', last: '۳ ساعت پیش',    initials: 'ن.ع', color: '#FF9F0A' },
  { name: 'فرهاد تهرانی',    email: 'f.tehrani@yazd-steel.com',        role: 'بیننده',     dept: 'مالی',      status: 'active', last: '۱ روز پیش',     initials: 'ف.ت', color: '#FF453A' },
  { name: 'مریم صادقی',    email: 'm.sadeghi@yazd-steel.com',        role: 'معامله‌گر',  dept: 'معاملات',     status: 'invite', last: 'دعوت ارسال شد',initials: 'م.ص', color: '#636366' },
];

const roleConfig: Record<string, { color: string; icon: React.ElementType }> = {
  'مالک':  { color: '#FF9F0A', icon: Crown  },
  'مدیر':  { color: '#BF5AF2', icon: Shield },
  'معامله‌گر': { color: '#0A84FF', icon: User   },
  'خریدار':  { color: '#30D158', icon: User   },
  'بیننده': { color: '#636366', icon: User   },
};

const inviteEmail = '';

export default function TeamPage() {
  const [search, setSearch] = useState('');
  const filtered = members.filter(m => m.name.toLowerCase().includes(search.toLowerCase()) || m.email.toLowerCase().includes(search.toLowerCase()));

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
        <div>
          <h1 style={{ fontSize: '28px', fontWeight: 700, letterSpacing: '-0.03em' }}>مدیریت تیم</h1>
          <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px' }}>اعضای تیم را دعوت کنید و نقش‌ها و دسترسی‌ها را مدیریت کنید</p>
        </div>
        <button style={{ display: 'flex', alignItems: 'center', gap: '8px', padding: '10px 20px', borderRadius: '12px', background: 'var(--color-brand-primary)', color: '#fff', border: 'none', fontFamily: 'inherit', fontSize: '14px', fontWeight: 600, cursor: 'pointer', boxShadow: '0 4px 12px rgba(10,132,255,0.30)' }}>
          <UserPlus size={16} /> دعوت عضو
        </button>
      </div>

      {/* Invite card */}
      <div style={{ background: 'linear-gradient(135deg, rgba(10,132,255,0.12), rgba(191,90,242,0.08))', borderRadius: '18px', padding: '24px', border: '1px solid rgba(10,132,255,0.20)', display: 'flex', gap: '16px', alignItems: 'center' }}>
        <Mail size={28} style={{ color: 'var(--color-brand-primary)', flexShrink: 0 }} />
        <div style={{ flex: 1 }}>
          <p style={{ fontSize: '15px', fontWeight: 700, color: 'var(--text-primary)' }}>دعوت از طریق ایمیل</p>
          <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '2px' }}>اعضای تیم ایمیلی برای پیوستن به سازمان شما دریافت می‌کنند</p>
        </div>
        <div style={{ display: 'flex', gap: '8px', alignItems: 'center' }}>
          <input placeholder="colleague@company.com" style={{ height: '42px', padding: '0 14px', width: '260px', border: '1.5px solid var(--border-default)', borderRadius: '12px', fontFamily: 'inherit', fontSize: '14px', background: 'var(--bg-surface)', color: 'var(--text-primary)', outline: 'none' }} />
          <button style={{ height: '42px', padding: '0 20px', borderRadius: '12px', background: 'var(--color-brand-primary)', border: 'none', color: '#fff', fontFamily: 'inherit', fontSize: '14px', fontWeight: 600, cursor: 'pointer' }}>ارسال دعوت</button>
        </div>
      </div>

      {/* Members table */}
      <div style={{ background: 'var(--bg-card)', borderRadius: '18px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', overflow: 'hidden' }}>
        <div style={{ padding: '20px 24px', borderBottom: '1px solid var(--border-subtle)', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ fontSize: '16px', fontWeight: 700 }}>اعضا ({members.length})</h2>
          <div style={{ position: 'relative' }}>
            <Search size={14} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-tertiary)' }} />
            <input value={search} onChange={e => setSearch(e.target.value)} placeholder="جستجوی اعضا…" style={{ height: '36px', padding: '0 12px 0 34px', width: '200px', border: '1.5px solid var(--border-default)', borderRadius: '10px', fontFamily: 'inherit', fontSize: '13px', background: 'var(--bg-input)', color: 'var(--text-primary)', outline: 'none' }} />
          </div>
        </div>
        <table style={{ width: '100%', borderCollapse: 'collapse' }}>
          <thead>
            <tr style={{ background: 'var(--bg-input)' }}>
              {['عضو', 'نقش', 'بخش', 'وضعیت', 'آخرین فعالیت', ''].map(h => (
                <th key={h} style={{ padding: '11px 20px', fontSize: '11px', fontWeight: 600, color: 'var(--text-tertiary)', textTransform: 'uppercase', letterSpacing: '0.06em', textAlign: 'left' }}>{h}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {filtered.map((m) => {
              const rc = roleConfig[m.role];
              const RIcon = rc.icon;
              return (
                <tr key={m.email} style={{ borderTop: '1px solid var(--border-subtle)' }}>
                  <td style={{ padding: '14px 20px' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                      <div style={{ width: '40px', height: '40px', borderRadius: '50%', background: m.color, display: 'flex', alignItems: 'center', justifyContent: 'center', color: '#fff', fontSize: '14px', fontWeight: 700, flexShrink: 0 }}>{m.initials}</div>
                      <div>
                        <p style={{ fontSize: '14px', fontWeight: 600, color: 'var(--text-primary)' }}>{m.name}</p>
                        <p style={{ fontSize: '12px', color: 'var(--text-tertiary)' }}>{m.email}</p>
                      </div>
                    </div>
                  </td>
                  <td style={{ padding: '14px 20px' }}>
                    <span style={{ padding: '4px 12px', borderRadius: '999px', background: `${rc.color}18`, color: rc.color, fontSize: '12px', fontWeight: 600, display: 'inline-flex', alignItems: 'center', gap: '5px' }}>
                      <RIcon size={11} /> {m.role}
                    </span>
                  </td>
                  <td style={{ padding: '14px 20px', fontSize: '14px', color: 'var(--text-secondary)' }}>{m.dept}</td>
                  <td style={{ padding: '14px 20px' }}>
                    <span style={{ padding: '3px 10px', borderRadius: '999px', background: m.status === 'active' ? '#30D15820' : '#FF9F0A20', color: m.status === 'active' ? '#30D158' : '#FF9F0A', fontSize: '12px', fontWeight: 600 }}>
                      {m.status === 'active' ? 'فعال' : 'در انتظار'}
                    </span>
                  </td>
                  <td style={{ padding: '14px 20px', fontSize: '13px', color: 'var(--text-tertiary)' }}>{m.last}</td>
                  <td style={{ padding: '14px 20px' }}>
                    <button style={{ padding: '6px', borderRadius: '8px', background: 'var(--bg-input)', border: 'none', cursor: 'pointer', color: 'var(--text-secondary)', display: 'flex' }}><MoreHorizontal size={14} /></button>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
    </div>
  );
}
