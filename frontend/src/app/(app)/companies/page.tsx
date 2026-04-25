'use client';
import { useState } from 'react';
import { Search, Globe, Star, CheckCircle, Building2, Users, MapPin, ExternalLink } from 'lucide-react';

const companies = [
  { name: 'Baosteel Group',        type: 'خریدار',           country: '🇨🇳 چین',         industry: 'فولاد',      employees: '120,000+', trades: 845, rating: 4.9, verified: true,  color: '#0A84FF' },
  { name: 'SABIC',                 type: 'فروشنده',           country: '🇸🇦 عربستان',  industry: 'شیمیایی',   employees: '33,000+',  trades: 612, rating: 4.8, verified: true,  color: '#30D158' },
  { name: 'Codelco Chile',         type: 'فروشنده',           country: '🇨🇱 شیلی',          industry: 'معدن',      employees: '29,000+',  trades: 531, rating: 4.7, verified: true,  color: '#FF9F0A' },
  { name: 'Norsk Hydro',           type: 'فروشنده',           country: '🇳🇴 نروژ',         industry: 'آلومینیوم',  employees: '35,000+',  trades: 489, rating: 4.9, verified: true,  color: '#BF5AF2' },
  { name: 'Vale S.A.',             type: 'فروشنده',           country: '🇧🇷 برزیل',         industry: 'معدن',      employees: '66,000+',  trades: 720, rating: 4.6, verified: true,  color: '#FF453A' },
  { name: 'IOCL India',            type: 'خریدار',           country: '🇮🇳 هند',          industry: 'انرژی',     employees: '33,000+',  trades: 380, rating: 4.5, verified: true,  color: '#FF9F0A' },
  { name: 'فولاد یزد',          type: 'خریدار و فروشنده', country: '🇮🇷 ایران',        industry: 'فولاد',      employees: '8,200+',   trades: 243, rating: 4.7, verified: true,  color: '#0A84FF' },
  { name: 'Teck Resources',        type: 'فروشنده',           country: '🇨🇦 کانادا',         industry: 'معدن',      employees: '13,000+',  trades: 305, rating: 4.6, verified: false, color: '#636366' },
];

const typeColor: Record<string, string> = {
  'خریدار': '#0A84FF', 'فروشنده': '#30D158', 'خریدار و فروشنده': '#BF5AF2',
};

export default function CompaniesPage() {
  const [search, setSearch] = useState('');
  const [filter, setFilter] = useState('all');
  const filtered = companies
    .filter(c => c.name.toLowerCase().includes(search.toLowerCase()))
    .filter(c => filter === 'همه' || c.type.includes(filter));

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
        <div>
          <h1 style={{ fontSize: '28px', fontWeight: 700, letterSpacing: '-0.03em' }}>دایرکتوری شرکت‌ها</h1>
          <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px' }}>شرکای تجاری تأییدشده از 180+ کشور</p>
        </div>
        <div style={{ display: 'flex', gap: '12px', alignItems: 'center' }}>
          <div style={{ position: 'relative' }}>
            <Search size={14} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-tertiary)' }} />
            <input value={search} onChange={e => setSearch(e.target.value)} placeholder="جستجوی شرکت‌ها…" style={{ height: '40px', padding: '0 12px 0 34px', width: '240px', border: '1.5px solid var(--border-default)', borderRadius: '12px', fontFamily: 'inherit', fontSize: '14px', color: 'var(--text-primary)', background: 'var(--bg-input)', outline: 'none' }} />
          </div>
        </div>
      </div>

      {/* Filter tabs */}
      <div style={{ display: 'flex', gap: '8px' }}>
        {['همه', 'خریدار', 'فروشنده'].map(f => (
          <button key={f} onClick={() => setFilter(f)} style={{ padding: '7px 18px', borderRadius: '999px', border: `1.5px solid ${filter === f ? 'var(--color-brand-primary)' : 'var(--border-default)'}`, background: filter === f ? 'var(--color-brand-primary)' : 'transparent', color: filter === f ? '#fff' : 'var(--text-secondary)', fontFamily: 'inherit', fontSize: '13px', fontWeight: 600, cursor: 'pointer', transition: 'all 150ms' }}>
            {f === 'همه' ? 'همه شرکت‌ها' : f === 'خریدار' ? 'خریداران' : 'فروشندگان'}
          </button>
        ))}
      </div>

      {/* Companies grid */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(340px, 1fr))', gap: '16px' }}>
        {filtered.map((c) => (
          <div key={c.name} style={{ background: 'var(--bg-card)', borderRadius: '20px', padding: '24px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', transition: 'transform 200ms ease, box-shadow 200ms ease', cursor: 'pointer' }}>
            {/* Logo placeholder + name */}
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: '16px' }}>
              <div style={{ display: 'flex', gap: '12px', alignItems: 'center' }}>
                <div style={{ width: '52px', height: '52px', borderRadius: '14px', background: `${c.color}18`, border: `1.5px solid ${c.color}30`, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                  <Building2 size={24} style={{ color: c.color }} />
                </div>
                <div>
                  <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
                    <p style={{ fontSize: '15px', fontWeight: 700, color: 'var(--text-primary)' }}>{c.name}</p>
                    {c.verified && <CheckCircle size={14} style={{ color: '#30D158', flexShrink: 0 }} />}
                  </div>
                  <span style={{ fontSize: '11px', fontWeight: 600, padding: '2px 8px', borderRadius: '999px', background: `${typeColor[c.type]}18`, color: typeColor[c.type], marginTop: '3px', display: 'inline-block' }}>{c.type}</span>
                </div>
              </div>
              <button style={{ padding: '7px', borderRadius: '10px', background: 'var(--bg-input)', border: 'none', cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--text-tertiary)' }}><ExternalLink size={14} /></button>
            </div>

            {/* Details */}
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '10px' }}>
              {[
                { icon: Globe,     label: 'کشور',  value: c.country    },
                { icon: Building2, label: 'صنعت', value: c.industry   },
                { icon: Users,     label: 'کارمندان',value: c.employees  },
                { icon: MapPin,    label: 'معاملات',   value: `${c.trades} معامله` },
              ].map((item) => {
                const Icon = item.icon;
                return (
                  <div key={item.label} style={{ display: 'flex', gap: '8px', alignItems: 'center' }}>
                    <Icon size={13} style={{ color: 'var(--text-tertiary)', flexShrink: 0 }} />
                    <div>
                      <p style={{ fontSize: '10px', color: 'var(--text-tertiary)', fontWeight: 500, textTransform: 'uppercase', letterSpacing: '0.05em' }}>{item.label}</p>
                      <p style={{ fontSize: '13px', fontWeight: 600, color: 'var(--text-secondary)' }}>{item.value}</p>
                    </div>
                  </div>
                );
              })}
            </div>

            {/* Rating */}
            <div style={{ marginTop: '16px', paddingTop: '14px', borderTop: '1px solid var(--border-subtle)', display: 'flex', alignItems: 'center', gap: '6px' }}>
              <div style={{ display: 'flex', gap: '2px' }}>
                {[1,2,3,4,5].map(i => <Star key={i} size={13} fill={i <= Math.floor(c.rating) ? '#FF9F0A' : 'none'} style={{ color: '#FF9F0A' }} />)}
              </div>
              <span style={{ fontSize: '13px', fontWeight: 700, color: 'var(--text-primary)' }}>{c.rating}</span>
              <span style={{ fontSize: '12px', color: 'var(--text-tertiary)' }}>· {c.trades} نظر</span>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
