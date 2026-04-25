'use client';
import { useState } from 'react';
import { MessageCircle, Send, Paperclip, CheckCircle2, Clock, XCircle } from 'lucide-react';

const threads = [
  { id: 1, counterpart: 'Baosteel Group',      initials: 'BG', color: '#0A84FF', product: 'Iron Ore 62% Fe',    lastMsg: 'We can offer CIF Shanghai at $116/MT.',  time: '2m ago',   unread: 3,  status: 'active'  },
  { id: 2, counterpart: 'SABIC Petrochemicals', initials: 'SP', color: '#30D158', product: 'HDPE Pellets',       lastMsg: 'Documents attached. Please review.',      time: '1h ago',   unread: 0,  status: 'active'  },
  { id: 3, counterpart: 'Codelco Chile',        initials: 'CC', color: '#FF9F0A', product: 'Copper Cathode',    lastMsg: 'Counter offer: $8,380/MT. Final.',        time: '3h ago',   unread: 1,  status: 'pending' },
  { id: 4, counterpart: 'Norsk Hydro',          initials: 'NH', color: '#BF5AF2', product: 'Aluminium Ingots',  lastMsg: 'Thank you, deal confirmed.',              time: '1d ago',   unread: 0,  status: 'closed'  },
  { id: 5, counterpart: 'Vale S.A.',            initials: 'VS', color: '#FF453A', product: 'Iron Ore 63% Fe',   lastMsg: 'We cannot accept below $118/MT.',         time: '2d ago',   unread: 0,  status: 'closed'  },
];

const messages = [
  { id: 1, sender: 'Baosteel Group',  side: 'left',  text: 'Hello, we are interested in your Iron Ore 62% Fe listing. Can you provide CIF pricing?',    time: '10:02' },
  { id: 2, sender: 'You',             side: 'right', text: 'Hi! For 2,000 MT, our CIF Shanghai is $118/MT. Shipment within 15 days.',                   time: '10:15' },
  { id: 3, sender: 'Baosteel Group',  side: 'left',  text: 'We appreciate the offer. Could you consider $116/MT? We can commit to 5,000 MT monthly.',    time: '10:28' },
  { id: 4, sender: 'You',             side: 'right', text: 'For 5,000 MT/month committed volume, I can go to $116.50/MT. We also need a 3-month contract.',time: '10:45' },
  { id: 5, sender: 'Baosteel Group',  side: 'left',  text: 'We can offer CIF Shanghai at $116/MT. Final offer for 3-month commitment.',                   time: '11:02' },
];

const statusColor: Record<string, string> = { active: '#30D158', pending: '#FF9F0A', closed: '#8E8E93' };
const StatusIcon: Record<string, React.ElementType> = { active: CheckCircle2, pending: Clock, closed: XCircle };

export default function NegotiationsPage() {
  const [selected, setSelected] = useState(1);
  const [msg, setMsg] = useState('');
  const current = threads.find(t => t.id === selected)!;

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '16px', height: 'calc(100vh - 140px)' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <div>
          <h1 style={{ fontSize: '28px', fontWeight: 700, letterSpacing: '-0.03em' }}>Negotiations</h1>
          <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px' }}>Real-time chat and price negotiation with trading partners</p>
        </div>
        <span style={{ padding: '4px 14px', borderRadius: '999px', background: '#0A84FF20', color: '#0A84FF', fontSize: '13px', fontWeight: 600 }}>2 Active</span>
      </div>

      <div style={{ display: 'flex', gap: '0', flex: 1, background: 'var(--bg-card)', borderRadius: '18px', border: '1px solid var(--border-subtle)', boxShadow: 'var(--shadow-card)', overflow: 'hidden', minHeight: 0 }}>
        {/* Thread list */}
        <div style={{ width: '320px', borderRight: '1px solid var(--border-subtle)', overflowY: 'auto', flexShrink: 0 }}>
          <div style={{ padding: '16px', borderBottom: '1px solid var(--border-subtle)' }}>
            <p style={{ fontSize: '13px', fontWeight: 600, color: 'var(--text-secondary)' }}>Conversations ({threads.length})</p>
          </div>
          {threads.map((t) => {
            const SIcon = StatusIcon[t.status];
            return (
              <div key={t.id} onClick={() => setSelected(t.id)} style={{ padding: '16px', borderBottom: '1px solid var(--border-subtle)', cursor: 'pointer', background: selected === t.id ? 'var(--bg-active)' : 'transparent', transition: 'background 150ms' }}>
                <div style={{ display: 'flex', gap: '12px', alignItems: 'flex-start' }}>
                  <div style={{ width: '42px', height: '42px', borderRadius: '50%', background: t.color, display: 'flex', alignItems: 'center', justifyContent: 'center', color: '#fff', fontSize: '13px', fontWeight: 700, flexShrink: 0 }}>{t.initials}</div>
                  <div style={{ flex: 1, minWidth: 0 }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                      <p style={{ fontSize: '13px', fontWeight: 600, color: 'var(--text-primary)' }}>{t.counterpart}</p>
                      <p style={{ fontSize: '11px', color: 'var(--text-tertiary)' }}>{t.time}</p>
                    </div>
                    <p style={{ fontSize: '11px', color: 'var(--color-brand-primary)', fontWeight: 500, marginBottom: '4px' }}>{t.product}</p>
                    <p style={{ fontSize: '12px', color: 'var(--text-secondary)', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{t.lastMsg}</p>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginTop: '6px' }}>
                      <div style={{ display: 'flex', alignItems: 'center', gap: '4px' }}>
                        <SIcon size={10} style={{ color: statusColor[t.status] }} />
                        <span style={{ fontSize: '11px', color: statusColor[t.status], fontWeight: 500, textTransform: 'capitalize' }}>{t.status}</span>
                      </div>
                      {t.unread > 0 && <span style={{ padding: '2px 7px', borderRadius: '999px', background: 'var(--color-brand-primary)', color: '#fff', fontSize: '11px', fontWeight: 600 }}>{t.unread}</span>}
                    </div>
                  </div>
                </div>
              </div>
            );
          })}
        </div>

        {/* Chat area */}
        <div style={{ flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0 }}>
          {/* Header */}
          <div style={{ padding: '16px 20px', borderBottom: '1px solid var(--border-subtle)', display: 'flex', alignItems: 'center', gap: '12px' }}>
            <div style={{ width: '40px', height: '40px', borderRadius: '50%', background: current.color, display: 'flex', alignItems: 'center', justifyContent: 'center', color: '#fff', fontSize: '13px', fontWeight: 700, flexShrink: 0 }}>{current.initials}</div>
            <div>
              <p style={{ fontSize: '15px', fontWeight: 700, color: 'var(--text-primary)' }}>{current.counterpart}</p>
              <p style={{ fontSize: '12px', color: 'var(--color-brand-primary)' }}>{current.product}</p>
            </div>
          </div>

          {/* Messages */}
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px', display: 'flex', flexDirection: 'column', gap: '16px' }}>
            {messages.map((m) => (
              <div key={m.id} style={{ display: 'flex', justifyContent: m.side === 'right' ? 'flex-end' : 'flex-start' }}>
                <div style={{ maxWidth: '70%', background: m.side === 'right' ? 'var(--color-brand-primary)' : 'var(--bg-input)', borderRadius: m.side === 'right' ? '16px 16px 4px 16px' : '16px 16px 16px 4px', padding: '12px 16px' }}>
                  <p style={{ fontSize: '14px', color: m.side === 'right' ? '#fff' : 'var(--text-primary)', lineHeight: 1.5 }}>{m.text}</p>
                  <p style={{ fontSize: '11px', marginTop: '6px', color: m.side === 'right' ? 'rgba(255,255,255,0.6)' : 'var(--text-tertiary)', textAlign: 'right' }}>{m.time}</p>
                </div>
              </div>
            ))}
          </div>

          {/* Input */}
          <div style={{ padding: '16px 20px', borderTop: '1px solid var(--border-subtle)', display: 'flex', gap: '10px', alignItems: 'flex-end' }}>
            <button style={{ padding: '10px', borderRadius: '12px', background: 'var(--bg-input)', border: 'none', cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--text-secondary)', flexShrink: 0 }}><Paperclip size={16} /></button>
            <textarea value={msg} onChange={e => setMsg(e.target.value)} placeholder="Type your message…" rows={2} style={{ flex: 1, resize: 'none', border: '1.5px solid var(--border-default)', borderRadius: '14px', padding: '12px 16px', fontFamily: 'inherit', fontSize: '14px', color: 'var(--text-primary)', background: 'var(--bg-input)', outline: 'none', lineHeight: 1.5 }} />
            <button style={{ padding: '10px 20px', borderRadius: '12px', background: 'var(--color-brand-primary)', border: 'none', cursor: 'pointer', display: 'flex', alignItems: 'center', gap: '8px', color: '#fff', fontSize: '14px', fontWeight: 600, fontFamily: 'inherit', flexShrink: 0, boxShadow: '0 4px 12px rgba(10,132,255,0.30)' }}>
              <Send size={15} /> Send
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
