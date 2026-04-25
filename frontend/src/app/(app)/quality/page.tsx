'use client';
import { FlaskConical, Award, AlertTriangle, CheckCircle, ClipboardList, FileSearch } from 'lucide-react';

const inspections = [
  { id: 'QC-2025-0118', product: 'کاتد مس درجه A', emoji: '🟡', shipment: 'SHP-2025-0221', inspector: 'Bureau Veritas', location: 'کالاما، شیلی',    date: '۲۴ دی', grade: 'A',    cu: '99.99%', moisture: '0.01%', result: 'قبول‌شده',  report: true  },
  { id: 'QC-2025-0101', product: 'پلت HDPE 20/60',     emoji: '⚪', shipment: 'SHP-2025-0198', inspector: 'SGS Group',       location: 'جبیل، عربستان',   date: '۱۱ دی', grade: 'A+',   mfi: '0.35',  density: '0.956', result: 'قبول‌شده',  report: true  },
  { id: 'QC-2025-0089', product: 'سنگ آهن ۶۲٪ Fe',    emoji: '🟤', shipment: 'SHP-2025-0180', inspector: 'Intertek',        location: 'کاراجاس، برزیل',   date: '۵ دی',  grade: 'B',    fe: '61.8%',  moisture: '8.2%', result: 'در انتظار', report: false },
  { id: 'QC-2024-0412', product: 'روی گرید LME',       emoji: '🔷', shipment: 'SHP-2024-0880', inspector: 'Bureau Veritas', location: 'تریل، کانادا',    date: '۱۵ آبان', grade: 'A',    zn: '99.995%',moisture: '0.00%',result: 'قبول‌شده',  report: true  },
  { id: 'QC-2024-0391', product: 'اوره ۴۶٪ N گرانوله',   emoji: '🟤', shipment: 'SHP-2024-0851', inspector: 'SGS Group',       location: 'یوژنه، اوکراین',   date: '۱ آبان',  grade: 'C',    n: '45.2%',   moisture: '1.1%', result: 'رد‌شده',  report: true  },
];

const resultConfig: Record<string, { color: string; label: string; icon: React.ElementType }> = {
  'قبول‌شده':  { color: '#30D158', label: 'قبول‌شده',  icon: CheckCircle    },
  'در انتظار': { color: '#FF9F0A', label: 'در انتظار', icon: ClipboardList  },
  'رد‌شده':  { color: '#FF453A', label: 'رد‌شده',  icon: AlertTriangle  },
};

export default function QualityPage() {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
      <div>
        <h1 style={{ fontSize: '28px', fontWeight: 700, letterSpacing: '-0.03em' }}>بازرسی کیفیت</h1>
        <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px' }}>گواهینامه‌های کیفیت اشخاص ثالث و نتایج آزمایشگاه</p>
      </div>

      {/* Stats */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '16px' }}>
        {[
          { icon: CheckCircle,   label: 'قبول‌شده',  value: '38', color: '#30D158' },
          { icon: ClipboardList, label: 'در انتظار', value: '4',  color: '#FF9F0A' },
          { icon: AlertTriangle, label: 'رد‌شده',  value: '2',  color: '#FF453A' },
          { icon: Award,         label: 'میانگین درجه', value: 'A', color: '#BF5AF2' },
        ].map((s) => {
          const Icon = s.icon;
          return (
            <div key={s.label} style={{ background: 'var(--bg-card)', borderRadius: '18px', padding: '20px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', display: 'flex', alignItems: 'center', gap: '14px' }}>
              <div style={{ width: '44px', height: '44px', borderRadius: '13px', background: `${s.color}18`, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
                <Icon size={20} style={{ color: s.color }} />
              </div>
              <div>
                <p style={{ fontSize: '11px', color: 'var(--text-secondary)', fontWeight: 500, textTransform: 'uppercase', letterSpacing: '0.06em' }}>{s.label}</p>
                <p style={{ fontSize: '26px', fontWeight: 700, letterSpacing: '-0.03em', marginTop: '2px' }}>{s.value}</p>
              </div>
            </div>
          );
        })}
      </div>

      {/* Inspection records */}
      <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
        {inspections.map((ins) => {
          const rc = resultConfig[ins.result];
          const RIcon = rc.icon;
          return (
            <div key={ins.id} style={{ background: 'var(--bg-card)', borderRadius: '18px', padding: '20px 24px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', display: 'flex', alignItems: 'flex-start', gap: '16px' }}>
              <div style={{ width: '52px', height: '52px', borderRadius: '14px', background: 'var(--bg-input)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '24px', flexShrink: 0 }}>{ins.emoji}</div>
              <div style={{ flex: 1, minWidth: 0 }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', gap: '12px' }}>
                  <div>
                    <p style={{ fontSize: '15px', fontWeight: 700, color: 'var(--text-primary)' }}>{ins.product}</p>
                    <p style={{ fontSize: '12px', color: 'var(--text-tertiary)', marginTop: '2px' }}>
                      {ins.id} · {ins.shipment} · بازرسی توسط {ins.inspector}
                    </p>
                    <p style={{ fontSize: '12px', color: 'var(--text-tertiary)' }}>{ins.location} · {ins.date}</p>
                  </div>
                  <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-end', gap: '8px' }}>
                    <span style={{ padding: '5px 14px', borderRadius: '999px', background: `${rc.color}20`, color: rc.color, fontSize: '12px', fontWeight: 600, display: 'flex', alignItems: 'center', gap: '5px' }}>
                      <RIcon size={11} /> {rc.label}
                    </span>
                    <div style={{ width: '32px', height: '32px', borderRadius: '50%', background: `${rc.color}20`, border: `2px solid ${rc.color}`, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '13px', fontWeight: 800, color: rc.color }}>
                      {ins.grade}
                    </div>
                  </div>
                </div>

                {/* Key parameters */}
                <div style={{ display: 'flex', gap: '10px', marginTop: '12px', flexWrap: 'wrap' }}>
                  {Object.entries({ ...ins }).filter(([k]) => !['id','product','emoji','shipment','inspector','location','date','grade','result','report'].includes(k)).map(([k, v]) => (
                    <div key={k} style={{ padding: '6px 12px', borderRadius: '10px', background: 'var(--bg-input)', display: 'flex', gap: '6px', alignItems: 'center' }}>
                      <FlaskConical size={12} style={{ color: 'var(--text-tertiary)' }} />
                      <span style={{ fontSize: '12px', fontWeight: 600, color: 'var(--text-primary)', textTransform: 'uppercase' }}>{k}</span>
                      <span style={{ fontSize: '12px', color: 'var(--text-secondary)' }}>{v as string}</span>
                    </div>
                  ))}
                </div>

                {ins.report && (
                  <button style={{ marginTop: '12px', display: 'flex', alignItems: 'center', gap: '6px', padding: '7px 14px', borderRadius: '10px', background: 'var(--bg-input)', border: '1px solid var(--border-default)', fontFamily: 'inherit', fontSize: '12px', fontWeight: 600, color: 'var(--text-secondary)', cursor: 'pointer' }}>
                    <FileSearch size={13} /> مشاهده گزارش کامل
                  </button>
                )}
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}
