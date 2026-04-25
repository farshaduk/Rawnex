'use client';
import Link from 'next/link';
import { TrendingUp, TrendingDown, Plus, ArrowRight, BarChart2, Package, ShoppingCart, FileText } from 'lucide-react';
import styles from './page.module.scss';

const stats = [
  { label: 'Ø¯Ø±Ø¢Ù…Ø¯ Ú©Ù„',        value: 'Û².Û´ Ù…ÛŒÙ„ÛŒÙˆÙ†$',  change: '+Û±Û¸.Û²Ùª', dir: 'up',   icon: 'ðŸ’°', color: 'blue' },
  { label: 'Ø³ÙØ§Ø±Ø´Ø§Øª ÙØ¹Ø§Ù„',    value: 'Û±Û´Û·',          change: '+Ûµ',      dir: 'up',   icon: 'ðŸ“¦', color: 'green' },
  { label: 'Ø¯Ø±Ø®ÙˆØ§Ø³Øª Ù‚ÛŒÙ…Øª Ø¨Ø§Ø²', value: 'Û³Û¸',          change: '-Û³',      dir: 'down', icon: 'ðŸ“„', color: 'orange' },
  { label: 'Ù¾Ø±Ø¯Ø§Ø®Øªâ€ŒÙ‡Ø§ÛŒ Ù…Ø¹Ù„Ù‚',  value: 'Û¸Û¹Û° Ù‡Ø²Ø§Ø±$',   change: '+Û±Û².Û±Ùª', dir: 'up',   icon: 'ðŸ’³', color: 'purple' },
];

const recentOrders = [
  { id: 'ORD-2847', product: 'Ú©Ø§ØªØ¯ Ù…Ø³',        emoji: 'ðŸ”¶', qty: 'ÛµÛ° ØªÙ†', value: 'Û´Û²Û°,Û°Û°Û°$', status: 'Ø¯Ø± Ø­Ù…Ù„',       statusColor: 'blue' },
  { id: 'ORD-2846', product: 'Ø³Ù†Ú¯ Ø¢Ù‡Ù† Fe65Ùª', emoji: 'â›ï¸', qty: 'ÛµÛ°Û° ØªÙ†', value: 'Û¸Ûµ,Û°Û°Û°$',  status: 'ØªØ£ÛŒÛŒØ¯Ø´Ø¯Ù‡',    statusColor: 'green' },
  { id: 'ORD-2845', product: 'Ú¯Ø±Ø§Ù†ÙˆÙ„ HDPE',    emoji: 'ðŸ­', qty: 'Û²Û° ØªÙ†',  value: 'Û³Û²,Û°Û°Û°$',  status: 'Ø¯Ø± Ø§Ù†ØªØ¸Ø§Ø±',   statusColor: 'orange' },
  { id: 'ORD-2844', product: 'Ø§ÙˆØ±Ù‡ Û´Û¶Ùª',       emoji: 'ðŸŒ¾', qty: 'Û²Û°Û° ØªÙ†', value: 'Û±Û±Û°,Û°Û°Û°$', status: 'ØªÚ©Ù…ÛŒÙ„â€ŒØ´Ø¯Ù‡',  statusColor: 'gray' },
  { id: 'ORD-2843', product: 'Ø³ÙˆØ¯ Ø³ÙˆØ²Ø¢ÙˆØ±',     emoji: 'ðŸ§ª', qty: 'Û±Û° ØªÙ†',  value: 'Û±Û¸,ÛµÛ°Û°$',  status: 'Ø¯Ø± Ø§Ø®ØªÙ„Ø§Ù',  statusColor: 'red' },
];

const activity = [
  { icon: 'ðŸ“¦', color: 'blue',   text: <><strong>ORD-2847</strong> Ø§Ø² Ø¨Ù†Ø¯Ø± Ø´Ø§Ù†Ú¯Ù‡Ø§ÛŒ Ø­Ø±Ú©Øª Ú©Ø±Ø¯</>, time: 'Û² Ø¯Ù‚ÛŒÙ‚Ù‡ Ù¾ÛŒØ´' },
  { icon: 'ðŸ¤', color: 'green',  text: <><strong>Ø´Ø±Ú©Øª Ù¾Ø§Ø±Ø³â€ŒÙÙˆÙ„Ø§Ø¯</strong> Ù¾ÛŒØ´Ù†Ù‡Ø§Ø¯ Ø´Ù…Ø§ Ø¨Ø±Ø§ÛŒ ÛµÛ°Û° ØªÙ† Ø³Ù†Ú¯ Ø¢Ù‡Ù† Ø±Ø§ Ù¾Ø°ÛŒØ±ÙØª</>, time: 'Û±Û¸ Ø¯Ù‚ÛŒÙ‚Ù‡ Ù¾ÛŒØ´' },
  { icon: 'ðŸ’³', color: 'orange', text: <>Ø§Ù…Ø§Ù†Øªâ€ŒØ¯Ø§Ø±ÛŒ <strong>Û´Û²Û°,Û°Û°Û°$</strong> Ø¨Ø±Ø§ÛŒ ORD-2844 Ø¢Ø²Ø§Ø¯ Ø´Ø¯</>, time: 'Û± Ø³Ø§Ø¹Øª Ù¾ÛŒØ´' },
  { icon: 'ðŸ“©', color: 'purple', text: <><strong>Û³ Ø¯Ø±Ø®ÙˆØ§Ø³Øª Ù‚ÛŒÙ…Øª Ø¬Ø¯ÛŒØ¯</strong> Ù…Ø·Ø§Ø¨Ù‚ Ú©Ø§ØªØ§Ù„ÙˆÚ¯ Ø´Ù…Ø§</>, time: 'Û² Ø³Ø§Ø¹Øª Ù¾ÛŒØ´' },
  { icon: 'âš ï¸', color: 'red',    text: <>Ù‡Ø´Ø¯Ø§Ø± Ø¨Ø§Ø²Ø±Ø³ÛŒ Ú©ÛŒÙÛŒØª <strong>ORD-2843</strong> â€” Ù†ÛŒØ§Ø² Ø¨Ù‡ Ø§Ù‚Ø¯Ø§Ù…</>, time: 'Û³ Ø³Ø§Ø¹Øª Ù¾ÛŒØ´' },
];

const marketPrices = [
  { icon: 'ðŸ”¶', name: 'Ù…Ø³ Ø¯Ø±Ø¬Ù‡ A',       unit: 'Ø¯Ù„Ø§Ø±/ØªÙ†', price: 'Û¹,Û²Û´Û¸$', change: '+Û±.Û´Ùª', dir: 'up' },
  { icon: 'âš«', name: 'Ø³Ù†Ú¯ Ø¢Ù‡Ù† Fe62Ùª', unit: 'Ø¯Ù„Ø§Ø±/ØªÙ†', price: 'Û±Û±Û²$',    change: '-Û°.Û¶Ùª', dir: 'down' },
  { icon: 'â¬œ', name: 'Ø¢Ù„ÙˆÙ…ÛŒÙ†ÛŒÙˆÙ…',      unit: 'Ø¯Ù„Ø§Ø±/ØªÙ†', price: 'Û²,Û³Û±Û°$', change: '+Û°.Û¸Ùª', dir: 'up' },
  { icon: 'ðŸŒ¾', name: 'Ø§ÙˆØ±Ù‡ ÙÙ„Ù‡',       unit: 'Ø¯Ù„Ø§Ø±/ØªÙ†', price: 'Û³Û±Ûµ$',   change: '+Û².Û±Ùª', dir: 'up' },
  { icon: 'ðŸ­', name: 'HDPE ØªØ²Ø±ÛŒÙ‚ÛŒ',    unit: 'Ø¯Ù„Ø§Ø±/ØªÙ†', price: 'Û±,Û±Û¸Û°$', change: '-Û±.Û²Ùª', dir: 'down' },
];

const quickActions = [
  { label: 'Ø«Ø¨Øª Ø¢Ú¯Ù‡ÛŒ',        icon: 'ðŸ“‹', href: '/listings/new' },
  { label: 'Ø¯Ø±Ø®ÙˆØ§Ø³Øª Ù‚ÛŒÙ…Øª',   icon: 'ðŸ“', href: '/rfq/new' },
  { label: 'Ù…Ø´Ø§Ù‡Ø¯Ù‡ Ù…Ø²Ø§ÛŒØ¯Ù‡â€ŒÙ‡Ø§', icon: 'ðŸŽ¯', href: '/auctions' },
  { label: 'Ø§ÙØ²ÙˆØ¯Ù† Ø´Ø±Ú©Øª',    icon: 'ðŸ¢', href: '/companies/new' },
];

export default function DashboardPage() {
  return (
    <div className={styles.dashboard}>
      {/* Header */}
      <div className={styles['page-header']}>
        <div className={styles['page-header__left']}>
          <h1>ØµØ¨Ø­ Ø¨Ø®ÛŒØ±ØŒ Ø¹Ù„ÛŒ ðŸ‘‹</h1>
          <p>Ø®Ù„Ø§ØµÙ‡â€ŒØ§ÛŒ Ø§Ø² ÙˆØ¶Ø¹ÛŒØª Ù¾Ø±ØªÙÙˆÙ„ÛŒÙˆÛŒ Ù…Ø¹Ø§Ù…Ù„Ø§ØªÛŒ Ø´Ù…Ø§ Ø§Ù…Ø±ÙˆØ².</p>
        </div>
        <div className={styles['page-header__actions']}>
          <Link href="/listings/new" style={{ display: 'inline-flex', alignItems: 'center', gap: '6px', padding: '10px 20px', borderRadius: '12px', fontFamily: 'inherit', fontSize: '13px', fontWeight: 600, color: '#fff', background: 'var(--color-brand-primary)', textDecoration: 'none', boxShadow: '0 4px 14px rgba(10,132,255,0.30)' }}>
            <Plus size={15} /> Ø¢Ú¯Ù‡ÛŒ Ø¬Ø¯ÛŒØ¯
          </Link>
        </div>
      </div>

      {/* Stats */}
      <div className={styles['stats-grid']}>
        {stats.map((s) => (
          <div key={s.label} className={styles['stat-card']}>
            <div className={styles['stat-card__header']}>
              <span className={styles['stat-card__label']}>{s.label}</span>
              <div className={`${styles['stat-card__icon']} ${styles[`stat-card__icon--${s.color}`]}`}>
                <span style={{ fontSize: '18px' }}>{s.icon}</span>
              </div>
            </div>
            <div className={styles['stat-card__value']}>{s.value}</div>
            <div className={`${styles['stat-card__change']} ${styles[`stat-card__change--${s.dir}`]}`}>
              {s.dir === 'up' ? <TrendingUp size={14} /> : <TrendingDown size={14} />}
              {s.change} <span>Ù†Ø³Ø¨Øª Ø¨Ù‡ Ù…Ø§Ù‡ Ú¯Ø°Ø´ØªÙ‡</span>
            </div>
          </div>
        ))}
      </div>

      {/* Quick Actions */}
      <div className={styles.card}>
        <div className={styles['card__header']}>
          <span className={styles['card__title']}>Ø¯Ø³ØªØ±Ø³ÛŒ Ø³Ø±ÛŒØ¹</span>
        </div>
        <div className={styles['quick-actions']}>
          {quickActions.map((a) => (
            <Link key={a.label} href={a.href} className={styles['quick-action']}>
              <div className={styles['quick-action__icon']}>{a.icon}</div>
              <span className={styles['quick-action__label']}>{a.label}</span>
            </Link>
          ))}
        </div>
      </div>

      {/* Main Grid */}
      <div className={styles['main-grid']}>
        {/* Recent Orders */}
        <div className={styles.card}>
          <div className={styles['card__header']}>
            <span className={styles['card__title']}>Ø³ÙØ§Ø±Ø´Ø§Øª Ø§Ø®ÛŒØ±</span>
            <Link href="/orders" className={styles['card__action']}>Ù…Ø´Ø§Ù‡Ø¯Ù‡ Ù‡Ù…Ù‡ <ArrowRight size={14} style={{ display: 'inline', verticalAlign: 'middle' }} /></Link>
          </div>
          <div className={`${styles['order-row']} ${styles['order-row--header']}`}>
            <span>Ù…Ø­ØµÙˆÙ„</span>
            <span>Ù…Ù‚Ø¯Ø§Ø±</span>
            <span>Ø§Ø±Ø²Ø´</span>
            <span>ÙˆØ¶Ø¹ÛŒØª</span>
            <span></span>
          </div>
          {recentOrders.map((o) => (
            <div key={o.id} className={styles['order-row']}>
              <div className={styles['order-product']}>
                <div className={styles['order-product__avatar']}>{o.emoji}</div>
                <div>
                  <div className={styles['order-product__name']}>{o.product}</div>
                  <div className={styles['order-product__id']}>{o.id}</div>
                </div>
              </div>
              <span className={styles['order-qty']}>{o.qty}</span>
              <span className={styles['order-value']}>{o.value}</span>
              <span className={`${styles.badge} ${styles[`badge--${o.statusColor}`]}`}>{o.status}</span>
              <Link href={`/orders/${o.id}`} style={{ fontSize: '12px', color: 'var(--text-brand)', textDecoration: 'none', fontWeight: 500 }}>Ù…Ø´Ø§Ù‡Ø¯Ù‡ â†</Link>
            </div>
          ))}
        </div>

        {/* Right column */}
        <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-5)' }}>
          {/* Market Prices */}
          <div className={styles.card}>
            <div className={styles['card__header']}>
              <span className={styles['card__title']}>Ù‚ÛŒÙ…Øªâ€ŒÙ‡Ø§ÛŒ Ø¨Ø§Ø²Ø§Ø±</span>
              <Link href="/analytics" className={styles['card__action']}>Ø²Ù†Ø¯Ù‡</Link>
            </div>
            <div className={styles['market-list']}>
              {marketPrices.map((m) => (
                <div key={m.name} className={styles['market-item']}>
                  <div className={styles['market-item__left']}>
                    <div className={styles['market-item__icon']}>{m.icon}</div>
                    <div>
                      <div className={styles['market-item__name']}>{m.name}</div>
                      <div className={styles['market-item__unit']}>{m.unit}</div>
                    </div>
                  </div>
                  <div className={styles['market-item__right']}>
                    <div className={styles['market-item__price']}>{m.price}</div>
                    <div className={`${styles['market-item__change']} ${styles[`market-item__change--${m.dir}`]}`}>
                      {m.dir === 'up' ? 'â–²' : 'â–¼'} {m.change}
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>

          {/* Activity Feed */}
          <div className={styles.card}>
            <div className={styles['card__header']}>
              <span className={styles['card__title']}>ÙØ¹Ø§Ù„ÛŒØªâ€ŒÙ‡Ø§ÛŒ Ø§Ø®ÛŒØ±</span>
              <Link href="/notifications" className={styles['card__action']}>Ù‡Ù…Ù‡</Link>
            </div>
            <div className={styles['activity-feed']}>
              {activity.map((item, i) => (
                <div key={i} className={styles['activity-item']}>
                  <div className={`${styles['activity-item__icon']} ${styles[`activity-item__icon--${item.color}`]}`}>
                    {item.icon}
                  </div>
                  <div className={styles['activity-item__body']}>
                    <div className={styles['activity-item__text']}>{item.text}</div>
                    <div className={styles['activity-item__time']}>{item.time}</div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
