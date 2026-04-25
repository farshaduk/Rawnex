'use client';
import { MapPin, Truck, Ship, Package, CheckCircle, Clock, AlertCircle } from 'lucide-react';

const shipments = [
  {
    id: 'SHP-2025-0221', product: 'کاتد مس درجه A', emoji: '🟡',
    origin: 'بندر کالاما، شیلی', dest: 'بندرعباس، ایران',
    weight: '500 MT', vessel: 'MV Pacific Star', eta: '۱۵ بهمن',
    status: 'in_transit', progress: 65,
    steps: [
      { label: 'سفارش تأییدشد',     done: true,  date: '۲۰ دی' },
      { label: 'بارگیری انجام شد',     done: true,  date: '۲۵ دی' },
      { label: 'مبدأ ترک شد',     done: true,  date: '۲۸ دی' },
      { label: 'در حمل (دریایی)', done: true,  date: 'فعال'      },
      { label: 'به بندر رسید',    done: false, date: 'ߤ۱۵ بهمن'},
      { label: 'گمرک ترخیص شد',    done: false, date: '—'           },
      { label: 'تحویل داده شد',          done: false, date: '—'           },
    ],
  },
  {
    id: 'SHP-2025-0198', product: 'پلت HDPE 20/60', emoji: '⚪',
    origin: 'صنعتی جبیل، عربستان', dest: 'بندرعباس، ایران',
    weight: '200 MT', vessel: 'MV Gulf Carrier', eta: '۸ بهمن',
    status: 'customs', progress: 85,
    steps: [
      { label: 'سفارش تأییدشد', done: true,  date: '۱۰ دی' },
      { label: 'بارگیری انجام شد',  done: true,  date: '۱۲ دی' },
      { label: 'مبدأ ترک شد', done: true,  date: '۱۴ دی' },
      { label: 'در حمل',      done: true,  date: '۱۴–۲۸ دی'   },
      { label: 'به بندر رسید', done: true,  date: '۲۸ دی' },
      { label: 'گمرک ترخیص شد', done: false, date: 'در انتظار'     },
      { label: 'تحویل داده شد',  done: false, date: 'ߤ۸ بهمن' },
    ],
  },
];

const statusConfig: Record<string, { label: string; color: string; icon: React.ElementType }> = {
  in_transit: { label: 'در حمل',    color: '#0A84FF', icon: Ship     },
  customs:    { label: 'در گمرک',   color: '#FF9F0A', icon: AlertCircle },
  delivered:  { label: 'تحویل‌داده‌شده', color: '#30D158', icon: CheckCircle },
};

const stats = [
  { icon: Ship,        label: 'محموله‌های فعال', value: '8',   color: '#0A84FF' },
  { icon: Truck,       label: 'در گمرک',        value: '2',   color: '#FF9F0A' },
  { icon: CheckCircle, label: 'تحویل (۳۰ روز)',    value: '14',  color: '#30D158' },
  { icon: Package,     label: 'مجموع بار',        value: '3,200 MT', color: '#BF5AF2' },
];

export default function LogisticsPage() {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
      <div>
        <h1 style={{ fontSize: '28px', fontWeight: 700, letterSpacing: '-0.03em' }}>لجستیک و ردیابی</h1>
        <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px' }}>ردیابی محموله در لحظه و مدیریت بار</p>
      </div>

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
                <p style={{ fontSize: '22px', fontWeight: 700, letterSpacing: '-0.03em', marginTop: '2px' }}>{s.value}</p>
              </div>
            </div>
          );
        })}
      </div>

      <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
        {shipments.map((s) => {
          const sc = statusConfig[s.status];
          const SIcon = sc.icon;
          return (
            <div key={s.id} style={{ background: 'var(--bg-card)', borderRadius: '20px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', overflow: 'hidden' }}>
              {/* Header */}
              <div style={{ padding: '20px 24px', borderBottom: '1px solid var(--border-subtle)', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div style={{ display: 'flex', gap: '14px', alignItems: 'center' }}>
                  <div style={{ width: '52px', height: '52px', borderRadius: '14px', background: 'var(--bg-input)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '24px', flexShrink: 0 }}>{s.emoji}</div>
                  <div>
                    <p style={{ fontSize: '16px', fontWeight: 700, color: 'var(--text-primary)' }}>{s.product}</p>
                    <p style={{ fontSize: '12px', color: 'var(--text-tertiary)', marginTop: '2px' }}>{s.id} · {s.vessel} · {s.weight}</p>
                  </div>
                </div>
                <div style={{ textAlign: 'right' }}>
                  <span style={{ padding: '5px 14px', borderRadius: '999px', background: `${sc.color}20`, color: sc.color, fontSize: '12px', fontWeight: 600, display: 'flex', alignItems: 'center', gap: '5px' }}>
                    <SIcon size={12} /> {sc.label}
                  </span>
                  <p style={{ fontSize: '12px', color: 'var(--text-secondary)', marginTop: '6px' }}>زمان تخمینی: <strong>{s.eta}</strong></p>
                </div>
              </div>

              {/* Route */}
              <div style={{ padding: '16px 24px', borderBottom: '1px solid var(--border-subtle)', display: 'flex', alignItems: 'center', gap: '12px' }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: '6px', flex: 1 }}>
                  <MapPin size={14} style={{ color: '#30D158', flexShrink: 0 }} />
                  <span style={{ fontSize: '13px', fontWeight: 600, color: 'var(--text-primary)' }}>{s.origin}</span>
                </div>
                <div style={{ flex: 2, height: '2px', background: `linear-gradient(to right, #30D158, ${sc.color})`, borderRadius: '1px', position: 'relative' }}>
                  <div style={{ position: 'absolute', left: `${s.progress}%`, top: '50%', transform: 'translate(-50%, -50%)', width: '10px', height: '10px', borderRadius: '50%', background: sc.color, border: '2px solid var(--bg-card)', boxShadow: `0 0 0 3px ${sc.color}40` }} />
                </div>
                <div style={{ display: 'flex', alignItems: 'center', gap: '6px', flex: 1, justifyContent: 'flex-end' }}>
                  <span style={{ fontSize: '13px', fontWeight: 600, color: 'var(--text-primary)' }}>{s.dest}</span>
                  <MapPin size={14} style={{ color: '#FF453A', flexShrink: 0 }} />
                </div>
              </div>

              {/* Steps */}
              <div style={{ padding: '20px 24px', display: 'flex', justifyContent: 'space-between', overflowX: 'auto' }}>
                {s.steps.map((step, i) => (
                  <div key={i} style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '6px', flex: 1, position: 'relative' }}>
                    {i < s.steps.length - 1 && (
                      <div style={{ position: 'absolute', top: '11px', left: '50%', width: '100%', height: '2px', background: step.done ? '#30D158' : 'var(--border-default)', zIndex: 0 }} />
                    )}
                    <div style={{ width: '22px', height: '22px', borderRadius: '50%', background: step.done ? '#30D158' : 'var(--bg-input)', border: `2px solid ${step.done ? '#30D158' : 'var(--border-default)'}`, display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1, flexShrink: 0 }}>
                      {step.done && <CheckCircle size={12} style={{ color: '#fff' }} />}
                    </div>
                    <p style={{ fontSize: '10px', fontWeight: 600, color: step.done ? 'var(--text-primary)' : 'var(--text-tertiary)', textAlign: 'center', lineHeight: 1.3, maxWidth: '72px' }}>{step.label}</p>
                    <p style={{ fontSize: '10px', color: 'var(--text-tertiary)', textAlign: 'center' }}>{step.date}</p>
                  </div>
                ))}
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}
