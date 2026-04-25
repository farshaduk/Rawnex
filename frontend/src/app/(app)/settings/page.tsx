'use client';
import { useState } from 'react';
import { User, Building2, Shield, Bell, CreditCard, Globe, Key, ChevronRight } from 'lucide-react';

const sections = [
  { icon: User,       label: 'پروفایل',              id: 'profile'    },
  { icon: Building2,  label: 'شرکت',               id: 'company'    },
  { icon: Shield,     label: 'امنیت و MFA',         id: 'security'   },
  { icon: Bell,       label: 'اعلانات',           id: 'notifs'     },
  { icon: CreditCard, label: 'صورتحساب',           id: 'billing'    },
  { icon: Globe,      label: 'زبان و منطقه',      id: 'locale'     },
  { icon: Key,        label: 'کلیدهای API',       id: 'api'        },
];

export default function SettingsPage() {
  const [active, setActive] = useState('profile');

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
      <div>
        <h1 style={{ fontSize: '28px', fontWeight: 700, letterSpacing: '-0.03em' }}>تنظیمات</h1>
        <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px' }}>حساب، شرکت و ترجیحات پلتفرم خود را مدیریت کنید</p>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '240px 1fr', gap: '20px' }}>
        {/* Sidebar nav */}
        <div style={{ background: 'var(--bg-card)', borderRadius: '18px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', height: 'fit-content', overflow: 'hidden' }}>
          {sections.map((s, i) => {
            const Icon = s.icon;
            const isActive = active === s.id;
            return (
              <button key={s.id} onClick={() => setActive(s.id)} style={{ width: '100%', display: 'flex', alignItems: 'center', gap: '12px', padding: '14px 18px', borderTop: i > 0 ? '1px solid var(--border-subtle)' : 'none', background: isActive ? 'var(--bg-active)' : 'transparent', color: isActive ? 'var(--color-brand-primary)' : 'var(--text-secondary)', fontFamily: 'inherit', fontSize: '14px', fontWeight: isActive ? 600 : 500, cursor: 'pointer', border: 'none', textAlign: 'left', transition: 'all 150ms ease' }}>
                <Icon size={16} style={{ flexShrink: 0 }} />
                <span style={{ flex: 1 }}>{s.label}</span>
                <ChevronRight size={14} style={{ opacity: 0.5 }} />
              </button>
            );
          })}
        </div>

        {/* Content */}
        <div style={{ background: 'var(--bg-card)', borderRadius: '18px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', overflow: 'hidden' }}>
          {active === 'profile' && (
            <div>
              <div style={{ padding: '24px', borderBottom: '1px solid var(--border-subtle)' }}>
                <h2 style={{ fontSize: '18px', fontWeight: 700, letterSpacing: '-0.02em' }}>تنظیمات پروفایل</h2>
                <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px' }}>اطلاعات شخصی خود را به‌روز کنید</p>
              </div>
              <div style={{ padding: '24px', display: 'flex', flexDirection: 'column', gap: '20px' }}>
                {/* Avatar */}
                <div style={{ display: 'flex', alignItems: 'center', gap: '16px', padding: '20px', background: 'var(--bg-input)', borderRadius: '14px' }}>
                  <div style={{ width: '72px', height: '72px', borderRadius: '50%', background: 'linear-gradient(135deg, #0A84FF, #BF5AF2)', display: 'flex', alignItems: 'center', justifyContent: 'center', color: '#fff', fontSize: '24px', fontWeight: 700, flexShrink: 0 }}>ع.ک</div>
                  <div>
                    <h3 style={{ fontSize: '16px', fontWeight: 600, color: 'var(--text-primary)' }}>علی کریمی</h3>
                    <p style={{ fontSize: '13px', color: 'var(--text-secondary)' }}>ali.karimi@yazd-steel.com</p>
                    <button style={{ marginTop: '8px', padding: '6px 14px', borderRadius: '8px', background: 'var(--bg-surface)', border: '1px solid var(--border-default)', fontFamily: 'inherit', fontSize: '12px', fontWeight: 600, color: 'var(--text-secondary)', cursor: 'pointer' }}>آپلود عکس</button>
                  </div>
                </div>

                {[
                  { label: 'نام',    placeholder: 'علی',                    type: 'text' },
                  { label: 'نام خانوادگی',     placeholder: 'کریمی',                 type: 'text' },
                  { label: 'ایمیل',         placeholder: 'ali.karimi@yazd-steel.com', type: 'email' },
                  { label: 'تلفن',         placeholder: '‏+98 21 1234 5678',       type: 'tel' },
                  { label: 'عنوان شغلی',     placeholder: 'مدیر تدارکات',    type: 'text' },
                ].map((f) => (
                  <div key={f.label} style={{ display: 'flex', flexDirection: 'column', gap: '6px' }}>
                    <label style={{ fontSize: '13px', fontWeight: 500, color: 'var(--text-primary)' }}>{f.label}</label>
                    <input type={f.type} defaultValue={f.placeholder} style={{ height: '44px', border: '1.5px solid var(--border-default)', borderRadius: '12px', padding: '0 16px', fontFamily: 'inherit', fontSize: '14px', color: 'var(--text-primary)', background: 'var(--bg-surface)', outline: 'none' }} />
                  </div>
                ))}

                <div style={{ display: 'flex', gap: '12px', justifyContent: 'flex-end', borderTop: '1px solid var(--border-subtle)', paddingTop: '20px' }}>
                  <button style={{ padding: '10px 24px', borderRadius: '12px', background: 'var(--bg-input)', border: '1px solid var(--border-default)', fontFamily: 'inherit', fontSize: '14px', fontWeight: 600, color: 'var(--text-secondary)', cursor: 'pointer' }}>لغو</button>
                  <button style={{ padding: '10px 24px', borderRadius: '12px', background: 'var(--color-brand-primary)', color: '#fff', fontFamily: 'inherit', fontSize: '14px', fontWeight: 600, cursor: 'pointer', border: 'none', boxShadow: '0 4px 12px rgba(10,132,255,0.30)' }}>ذخیره تغییرات</button>
                </div>
              </div>
            </div>
          )}

          {active === 'security' && (
            <div>
              <div style={{ padding: '24px', borderBottom: '1px solid var(--border-subtle)' }}>
                <h2 style={{ fontSize: '18px', fontWeight: 700, letterSpacing: '-0.02em' }}>امنیت و MFA</h2>
              </div>
              <div style={{ padding: '24px', display: 'flex', flexDirection: 'column', gap: '16px' }}>
                {[
                  { label: 'احراز هویت دو مرحله‌ای', desc: 'افزودن لایه امنیتی اضافی از طریق TOTP', enabled: true },
                  { label: 'اعلانات پیامکی',         desc: 'هشدار بر روی تلفن ثبت‌شده',          enabled: true },
                  { label: 'هشدار ورود',              desc: 'ایمیل هشدار برای ورود دستگاه جدید',              enabled: false },
                  { label: 'ورود بیومتریک',           desc: 'استفاده از اثر انگشت یا تشخیص چهره',         enabled: false },
                ].map((item) => (
                  <div key={item.label} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '16px', background: 'var(--bg-input)', borderRadius: '14px' }}>
                    <div>
                      <p style={{ fontSize: '14px', fontWeight: 600, color: 'var(--text-primary)' }}>{item.label}</p>
                      <p style={{ fontSize: '12px', color: 'var(--text-secondary)', marginTop: '2px' }}>{item.desc}</p>
                    </div>
                    <div style={{ width: '44px', height: '26px', borderRadius: '999px', background: item.enabled ? 'var(--color-brand-primary)' : 'var(--bg-surface)', border: `1.5px solid ${item.enabled ? 'transparent' : 'var(--border-default)'}`, position: 'relative', cursor: 'pointer', transition: 'all 250ms ease', flexShrink: 0 }}>
                      <div style={{ position: 'absolute', top: '3px', left: item.enabled ? '20px' : '3px', width: '18px', height: '18px', borderRadius: '50%', background: '#fff', boxShadow: '0 1px 4px rgba(0,0,0,0.20)', transition: 'left 250ms ease' }} />
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}

          {active !== 'profile' && active !== 'security' && (
            <div style={{ padding: '48px', display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '12px', color: 'var(--text-tertiary)' }}>
              <span style={{ fontSize: '48px' }}>🔧</span>
              <p style={{ fontSize: '15px', fontWeight: 500 }}>بخش تنظیمات به‌زودی می‌آید</p>
              <p style={{ fontSize: '13px', textAlign: 'center', maxWidth: '280px' }}>این بخش در حال توسعه است. به زودی بررسی کنید.</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
