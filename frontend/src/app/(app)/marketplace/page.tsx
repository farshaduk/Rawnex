'use client';
import Link from 'next/link';
import { Grid, List, SlidersHorizontal, Plus, ArrowRight, Search } from 'lucide-react';
import { useState } from 'react';
import styles from './page.module.scss';

const categories = ['همه', 'فلزات', 'مواد شیمیایی', 'پلیمرها', 'کشاورزی', 'مواد معدنی', 'انرژی'];
const categoryMap: Record<string, string> = {
  'Metals': 'فلزات', 'Minerals': 'مواد معدنی', 'Polymers': 'پلیمرها',
  'Agricultural': 'کشاورزی', 'Chemicals': 'مواد شیمیایی', 'Energy': 'انرژی',
};

const products = [
  { id: 1, emoji: '🔶', category: 'Metals',     name: 'کاتد مس درجه A',          seller: 'پارس کاپر',             price: '$9,248', unit: '/MT',  badge: 'تأییدشده', badgeColor: 'green',  specs: ['99.99% Cu', 'LME Grade', 'Bulk'] },
  { id: 2, emoji: '⛏️', category: 'Minerals',   name: 'سنگ آهن ریزدانه Fe65%',   seller: 'چادرملو',               price: '$112',   unit: '/DMT', badge: 'فرصت ویژه', badgeColor: 'orange', specs: ['Fe 65%', 'Low P', 'ایران'] },
  { id: 3, emoji: '🏭', category: 'Polymers',   name: 'HDPE فیلم گرید BL3',       seller: 'پتروشیمی تبریز',        price: '$1,180', unit: '/MT',  badge: 'جدید',      badgeColor: 'new',    specs: ['MFI 0.3', 'Blow Film', 'Natural'] },
  { id: 4, emoji: '⬜', category: 'Metals',     name: 'شمش آلومینیوم 1050A',      seller: 'سالکو',                 price: '$2,310', unit: '/MT',  badge: 'تأییدشده', badgeColor: 'green',  specs: ['99.5% Al', 'T-Bar', 'LME'] },
  { id: 5, emoji: '🌾', category: 'Agricultural',name: 'اوره گرانوله ۴۶٪N',       seller: 'پتروشیمی شیراز',        price: '$315',   unit: '/MT',  badge: 'موجود',     badgeColor: 'blue',   specs: ['N 46%', 'SGN 280', 'Bulk/Bag'] },
  { id: 6, emoji: '🧪', category: 'Chemicals',  name: 'پرک سود سوزآور ۹۸٪',      seller: 'پتروشیمی بندر امام',    price: '$385',   unit: '/MT',  badge: 'تأییدشده', badgeColor: 'green',  specs: ['NaOH 98%', 'Flake', 'Drum/Bag'] },
  { id: 7, emoji: '🔵', category: 'Chemicals',  name: 'متانول صنعتی',             seller: 'پتروشیمی فناوران',      price: '$285',   unit: '/MT',  badge: 'فرصت ویژه', badgeColor: 'orange', specs: ['CH3OH 99.9%', 'IBC/Bulk', 'ACS'] },
  { id: 8, emoji: '🟡', category: 'Metals',     name: 'روی SHG — درجه ویژه',      seller: 'ذوب روی بافق',          price: '$2,680', unit: '/MT',  badge: 'جدید',      badgeColor: 'new',    specs: ['Zn 99.995%', 'LME Grade', 'Ingot'] },
];

export default function MarketplacePage() {
  const [activeCategory, setActiveCategory] = useState('همه');
  const [viewMode, setViewMode] = useState<'grid' | 'list'>('grid');

  const filtered = activeCategory === 'همه' ? products : products.filter(p => categoryMap[p.category] === activeCategory);

  return (
    <div className={styles.marketplace}>
      {/* Header */}
      <div className={styles.header}>
        <div className={styles['header__left']}>
          <h1>بازار محصولات</h1>
          <p>محصولات تأییدشده از تأمین‌کنندگان معتبر جهانی کشف کنید</p>
        </div>
        <Link href="/listings/new" style={{ display: 'inline-flex', alignItems: 'center', gap: '6px', padding: '10px 20px', borderRadius: '12px', fontFamily: 'inherit', fontSize: '13px', fontWeight: 600, color: '#fff', background: 'var(--color-brand-primary)', textDecoration: 'none', boxShadow: '0 4px 14px rgba(10,132,255,0.30)' }}>
          <Plus size={15} /> ثبت آگهی
        </Link>
      </div>

      {/* Filters */}
      <div className={styles.filters}>
        <div className={styles['filters__left']}>
          {categories.map((cat) => (
            <button
              key={cat}
              className={`${styles['filter-btn']} ${activeCategory === cat ? styles['filter-btn--active'] : ''}`}
              onClick={() => setActiveCategory(cat)}
            >
              {cat}
            </button>
          ))}
        </div>
        <div style={{ display: 'flex', gap: '8px', alignItems: 'center' }}>
          <select className={styles['filter-select']} aria-label="مرتب‌سازی">
            <option>جدیدترین</option>
            <option>قیمت: کم به زیاد</option>
            <option>قیمت: زیاد به کم</option>
            <option>محبوب‌ترین</option>
          </select>
          <div className={styles['view-toggle']}>
            <button className={`${styles['view-btn']} ${viewMode === 'grid' ? styles['view-btn--active'] : ''}`} onClick={() => setViewMode('grid')} aria-label="نمای شبکه‌ای">
              <Grid size={15} />
            </button>
            <button className={`${styles['view-btn']} ${viewMode === 'list' ? styles['view-btn--active'] : ''}`} onClick={() => setViewMode('list')} aria-label="نمای فهرستی">
              <List size={15} />
            </button>
          </div>
        </div>
      </div>

      {/* Product Grid */}
      <div className={styles['product-grid']} style={viewMode === 'list' ? { gridTemplateColumns: '1fr' } : undefined}>
        {filtered.map((p) => (
          <Link key={p.id} href={`/marketplace/${p.id}`} className={styles['product-card']}>
            <div className={styles['product-card__image']}>
              {p.emoji}
              <div className={styles['product-card__badge-top']}>
                <span className={`${styles.badge} ${styles[`badge--${p.badgeColor}`]}`}>{p.badge}</span>
              </div>
            </div>
            <div className={styles['product-card__body']}>
              <span className={styles['product-card__category']}>{categoryMap[p.category] || p.category}</span>
              <h3 className={styles['product-card__name']}>{p.name}</h3>
              <div className={styles['product-card__seller']}>
                <span className={styles['product-card__seller-dot']} />
                {p.seller}
              </div>
              <div className={styles['product-card__specs']}>
                {p.specs.map((s) => (
                  <span key={s} className={styles['spec-tag']}>{s}</span>
                ))}
              </div>
            </div>
            <div className={styles['product-card__footer']}>
              <div className={styles['product-card__price']}>
                {p.price} <span>{p.unit}</span>
              </div>
              <div className={styles['product-card__action']}>
                <ArrowRight size={15} />
              </div>
            </div>
          </Link>
        ))}
      </div>

      {/* Pagination */}
      <div className={styles.pagination}>
        {[1, 2, 3, '...', 8, 9].map((p, i) => (
          <button key={i} className={`${styles['page-btn']} ${p === 1 ? styles['page-btn--active'] : ''}`}>
            {p}
          </button>
        ))}
      </div>
    </div>
  );
}
