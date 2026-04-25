'use client';
import { useState } from 'react';
import { BarChart3, Users, Building2, ShieldCheck, AlertOctagon, Activity, TrendingUp, Globe, CheckCircle, XCircle, Clock } from 'lucide-react';

const kpis = [
  { icon: Users,      label: 'کاربران کل',      value: '48,291',  delta: '+1,204', trend: 'up',   color: '#0A84FF' },
  { icon: Building2,  label: 'شرکت‌ها',        value: '12,847',  delta: '+318',   trend: 'up',   color: '#30D158' },
  { icon: BarChart3,  label: 'حجم ماهانه',   value: '$412M',   delta: '+8.2%',  trend: 'up',   color: '#BF5AF2' },
  { icon: ShieldCheck,label: 'تقلب شناسایی‌شده', value: '7', delta: '-3', trend: 'down', color: '#FF453A' },
];

const pendingVerifications = [
  { name: 'شرکت آهن تهران',    country: '🇮🇷 ایران',   type: 'فروشنده', docs: 'CR, Tax, Trade',  submitted: '۲۸ دی ۱۴۰۳' },
  { name: 'SinoSteel Corp',     country: '🇨🇳 چین',  type: 'خریدار', docs: 'CR, Bank Letter', submitted: '۲۷ دی ۱۴۰۳' },
  { name: 'Boliden AB',         country: '🇸🇪 سوئد',  type: 'فروشنده', docs: 'CR, Tax, ISO',    submitted: '۲۶ دی ۱۴۰۳' },
  { name: 'Gulf Polymers DMCC', country: '🇦🇪 امارات',  type: 'معامله‌گر', docs: 'CR, Tax',      submitted: '۲۵ دی ۱۴۰۳' },
];

const recentAlerts = [
  { type: 'fraud',   msg: 'الگوی IP مشکوک برای کاربر uid_88821',         time: '۱۲ دقیقه پیش',  color: '#FF453A', icon: AlertOctagon },
  { type: 'login',   msg: 'تلاش‌های ورود مکرر: m.sadeghi@yazd-steel.com', time: '۱ ساعت پیش',   color: '#FF9F0A', icon: AlertOctagon },
  { type: 'large',   msg: 'تراکنش کلان: معامله $22M — CNT-2025-0110',    time: '۳ ساعت پیش',   color: '#0A84FF', icon: Activity     },
  { type: 'system',  msg: 'پشتیبان‌گیری پایگاه داده با موفقیت انجام شد',            time: '۶ ساعت پیش',   color: '#30D158', icon: CheckCircle  },
];

const regions = [
  { region: 'آسیا-اقیانوسیه', share: 38, color: '#0A84FF' },
  { region: 'خاورمیانه',  share: 27, color: '#30D158' },
  { region: 'اروپا',       share: 18, color: '#BF5AF2' },
  { region: 'آمریکاها',     share: 13, color: '#FF9F0A' },
  { region: 'آفریقا',       share: 4,  color: '#FF453A' },
];

export default function AdminPage() {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
        <div>
          <div style={{ display: 'inline-flex', alignItems: 'center', gap: '8px', padding: '4px 14px', borderRadius: '999px', background: 'rgba(255,69,58,0.14)', marginBottom: '8px' }}>
            <ShieldCheck size={13} style={{ color: '#FF453A' }} />
            <span style={{ fontSize: '12px', fontWeight: 700, color: '#FF453A', letterSpacing: '0.06em', textTransform: 'uppercase' }}>سوپر ادمین</span>
          </div>
          <h1 style={{ fontSize: '28px', fontWeight: 700, letterSpacing: '-0.03em' }}>پنل مدیریت</h1>
          <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px' }}>تحلیل کل پلتفرم، تأییدیه‌ها و پایش امنیتی</p>
        </div>
      </div>

      {/* KPI cards */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '16px' }}>
        {kpis.map((k) => {
          const Icon = k.icon;
          return (
            <div key={k.label} style={{ background: 'var(--bg-card)', borderRadius: '18px', padding: '20px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '12px' }}>
                <p style={{ fontSize: '11px', color: 'var(--text-secondary)', fontWeight: 500, textTransform: 'uppercase', letterSpacing: '0.06em' }}>{k.label}</p>
                <div style={{ width: '34px', height: '34px', borderRadius: '10px', background: `${k.color}18`, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                  <Icon size={16} style={{ color: k.color }} />
                </div>
              </div>
              <p style={{ fontSize: '28px', fontWeight: 700, letterSpacing: '-0.03em' }}>{k.value}</p>
              <div style={{ display: 'flex', alignItems: 'center', gap: '4px', marginTop: '4px' }}>
                <TrendingUp size={12} style={{ color: k.trend === 'up' ? '#30D158' : '#FF453A', transform: k.trend === 'down' ? 'rotate(180deg)' : 'none' }} />
                <span style={{ fontSize: '12px', color: k.trend === 'up' ? '#30D158' : '#FF453A', fontWeight: 600 }}>{k.delta}</span>
                <span style={{ fontSize: '12px', color: 'var(--text-tertiary)' }}>نسبت به ماه گذشته</span>
              </div>
            </div>
          );
        })}
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1.5fr 1fr', gap: '20px' }}>
        {/* Pending verifications */}
        <div style={{ background: 'var(--bg-card)', borderRadius: '18px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', overflow: 'hidden' }}>
          <div style={{ padding: '18px 20px', borderBottom: '1px solid var(--border-subtle)', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <div>
              <h2 style={{ fontSize: '15px', fontWeight: 700 }}>تأییدیه‌های در انتظار</h2>
              <p style={{ fontSize: '12px', color: 'var(--text-tertiary)' }}>شرکت‌های در انتظار تأیید</p>
            </div>
            <span style={{ padding: '3px 10px', borderRadius: '999px', background: '#FF9F0A20', color: '#FF9F0A', fontSize: '12px', fontWeight: 700 }}>{pendingVerifications.length} در انتظار</span>
          </div>
          <div>
            {pendingVerifications.map((v, i) => (
              <div key={v.name} style={{ padding: '14px 20px', borderBottom: i < pendingVerifications.length - 1 ? '1px solid var(--border-subtle)' : 'none', display: 'flex', alignItems: 'center', gap: '12px' }}>
                <div style={{ width: '40px', height: '40px', borderRadius: '12px', background: 'var(--bg-input)', display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
                  <Building2 size={18} style={{ color: 'var(--text-secondary)' }} />
                </div>
                <div style={{ flex: 1, minWidth: 0 }}>
                  <p style={{ fontSize: '14px', fontWeight: 600, color: 'var(--text-primary)' }}>{v.name}</p>
                  <p style={{ fontSize: '12px', color: 'var(--text-tertiary)' }}>{v.country} · {v.type} · {v.docs}</p>
                </div>
                <div style={{ display: 'flex', gap: '6px' }}>
                  <button style={{ padding: '6px 14px', borderRadius: '9px', background: '#30D15820', border: '1px solid #30D15840', color: '#30D158', fontFamily: 'inherit', fontSize: '12px', fontWeight: 600, cursor: 'pointer', display: 'flex', alignItems: 'center', gap: '4px' }}><CheckCircle size={12} /> تأیید</button>
                  <button style={{ padding: '6px 14px', borderRadius: '9px', background: '#FF453A18', border: '1px solid #FF453A30', color: '#FF453A', fontFamily: 'inherit', fontSize: '12px', fontWeight: 600, cursor: 'pointer', display: 'flex', alignItems: 'center', gap: '4px' }}><XCircle size={12} /> رد</button>
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Security alerts */}
        <div style={{ background: 'var(--bg-card)', borderRadius: '18px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', overflow: 'hidden' }}>
          <div style={{ padding: '18px 20px', borderBottom: '1px solid var(--border-subtle)' }}>
            <h2 style={{ fontSize: '15px', fontWeight: 700 }}>هشدارهای امنیتی</h2>
            <p style={{ fontSize: '12px', color: 'var(--text-tertiary)' }}>پایش تهدید بلادرنگ</p>
          </div>
          <div style={{ padding: '12px' }}>
            {recentAlerts.map((a, i) => {
              const Icon = a.icon;
              return (
                <div key={i} style={{ display: 'flex', gap: '10px', padding: '10px', borderRadius: '12px', marginBottom: '4px' }}>
                  <div style={{ width: '32px', height: '32px', borderRadius: '10px', background: `${a.color}18`, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
                    <Icon size={14} style={{ color: a.color }} />
                  </div>
                  <div style={{ flex: 1, minWidth: 0 }}>
                    <p style={{ fontSize: '13px', fontWeight: 500, color: 'var(--text-primary)', lineHeight: 1.4 }}>{a.msg}</p>
                    <p style={{ fontSize: '11px', color: 'var(--text-tertiary)', marginTop: '2px' }}>{a.time}</p>
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      </div>

      {/* Regional breakdown */}
      <div style={{ background: 'var(--bg-card)', borderRadius: '18px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', padding: '20px 24px' }}>
        <h2 style={{ fontSize: '15px', fontWeight: 700, marginBottom: '16px' }}>حجم معاملات بر اساس منطقه</h2>
        <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
          {regions.map((r) => (
            <div key={r.region} style={{ display: 'flex', alignItems: 'center', gap: '14px' }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: '6px', width: '130px', flexShrink: 0 }}>
                <Globe size={12} style={{ color: r.color }} />
                <span style={{ fontSize: '13px', fontWeight: 500, color: 'var(--text-primary)' }}>{r.region}</span>
              </div>
              <div style={{ flex: 1, height: '8px', borderRadius: '4px', background: 'var(--bg-input)' }}>
                <div style={{ width: `${r.share}%`, height: '100%', borderRadius: '4px', background: r.color, transition: 'width 600ms ease' }} />
              </div>
              <span style={{ fontSize: '13px', fontWeight: 700, color: 'var(--text-primary)', width: '36px', textAlign: 'right', flexShrink: 0 }}>{r.share}%</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
