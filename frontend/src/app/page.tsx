import Link from 'next/link';
import styles from './page.module.scss';
import { Zap, ArrowRight, ChevronRight } from 'lucide-react';

const features = [
  { icon: '⚡', class: 'blue',   title: 'تطابق هوشمند با هوش مصنوعی',       desc: 'موتور هوش مصنوعی ما فوری بهترین خریداران و فروشندگان را بر اساس مشخصات محصول، قیمت‌گذاری و سابقه معاملاتی به شما معرفی می‌کند.' },
  { icon: '🔒', class: 'green',  title: 'امانت‌داری و پرداخت‌های امن',  desc: 'امانت‌داری مرحله‌ای هر معامله را حفاظت می‌کند. وجوه فقط پس از تحویل و تأیید کیفیت آزاد می‌شود.' },
  { icon: '📊', class: 'purple', title: 'تحلیل بلادرنگ',       desc: 'هوش بازار زنده، پیش‌بینی قیمت و سیگنال تقاضا برای برتری رقابتی در هر معامله.' },
  { icon: '🚚', class: 'orange', title: 'لجستیک کامل تا تحویل',      desc: 'تتبع محموله‌ها به صورت بلادرنگ، مدیریت اسناد و پشتیبانی گمرکی — همه از یک داشبورد.' },
  { icon: '🛡️', class: 'red',    title: 'جلوگیری از تقلب',          desc: 'KYC/KYB چندلایه، غربالگری تحریم، احراز بیومتریک و تشخیص ناهنجاری هوش مصنوعی برای هر حساب.' },
  { icon: '🌐', class: 'teal',   title: 'تنانتیدی جهانی',       desc: 'جداسازی کامل داده به ازای شرکت، پشتیبانی چندزبانه و RTL برای بازارهای فارسی و عربی.' },
];

const marqueeItems = [
  'Copper · LME Grade A', 'Iron Ore · Fe 65%', 'Polypropylene · PP Injection',
  'Urea · Bulk Granular', 'Soybean Oil · Crude', 'Aluminium Ingot · 1050A',
  'Zinc Concentrate', 'Palm Stearin · RBD', 'Caustic Soda · Flake',
  'Polyethylene · HDPE', 'Chrome Ore · Lumpy', 'Methanol · ACS Grade',
];

export default function LandingPage() {
  return (
    <>
      {/* Navbar */}
      <nav className={styles.navbar}>
        <Link href="/" className={styles.navbar__logo}>
          <div className={styles['navbar__logo-icon']}>R</div>
          <span className={styles.navbar__brand}>Raw<span>nex</span></span>
        </Link>
        <div className={styles.navbar__links}>
          {['ویژگی‌ها', 'راه‌حل‌ها', 'قیمت‌گذاری', 'درباره ما'].map((item, idx) => {
            const hrefs = ['features', 'solutions', 'pricing', 'about'];
            return (
              <Link key={item} href={`#${hrefs[idx]}`} className={styles.navbar__link}>{item}</Link>
            );
          })}
        </div>
        <div className={styles.navbar__actions}>
          <Link href="/auth/login" style={{ padding: '8px 18px', borderRadius: '10px', fontSize: '13px', fontWeight: 500, color: 'var(--text-secondary)', textDecoration: 'none' }}>
            ورود
          </Link>
          <Link href="/auth/register" style={{ padding: '8px 20px', borderRadius: '10px', fontSize: '13px', fontWeight: 600, color: '#fff', background: 'var(--color-brand-primary)', textDecoration: 'none', boxShadow: '0 4px 14px rgba(10,132,255,0.35)' }}>
            شروع کنید
          </Link>
        </div>
      </nav>

      {/* Hero */}
      <section className={styles.hero}>
        <div className={styles['hero__bg-grid']} aria-hidden="true" />
        <div className={styles.hero__content}>
          <div className={styles.hero__badge}>
            <Zap size={14} />
            با هوش مصنوعی · امنیت سازمانی
            <ChevronRight size={13} />
          </div>
          <h1 className={styles.hero__title}>
            معامله مواد اولیه<br />
            <span className={styles.gradient}>هوشمندتر. سریع‌تر. امن‌تر.</span>
          </h1>
          <p className={styles.hero__subtitle}>
            پیشرفته‌ترین پلتفرم معاملات B2B مواد اولیه جهان. تطابق هوشمند،
            پرداخت امانت‌داری، لجستیک بلادرنگ و مدیریت کامل چرخه معاملاتی.
          </p>
          <div className={styles.hero__actions}>
            <Link href="/auth/register" style={{ display: 'inline-flex', alignItems: 'center', gap: '8px', padding: '14px 32px', borderRadius: '14px', fontFamily: 'inherit', fontSize: '15px', fontWeight: 600, color: '#fff', background: 'var(--color-brand-primary)', textDecoration: 'none', boxShadow: '0 4px 24px rgba(10,132,255,0.40)' }}>
              شروع معامله رایگان <ArrowRight size={16} />
            </Link>
            <Link href="/dashboard" style={{ display: 'inline-flex', alignItems: 'center', gap: '8px', padding: '14px 28px', borderRadius: '14px', fontFamily: 'inherit', fontSize: '15px', fontWeight: 600, color: 'var(--text-primary)', background: 'rgba(255,255,255,0.72)', backdropFilter: 'blur(12px)', textDecoration: 'none', border: '1px solid var(--border-default)' }}>
              مشاهده دمو
            </Link>
          </div>
          <div className={styles.hero__stats}>
            {[
              { value: '$4.8B+', label: 'حجم سالانه معاملات' },
              { value: '12,000+', label: 'شرکت تأییدشده' },
              { value: '180+', label: 'کشور' },
              { value: '99.97%', label: 'آپ‌تایم پلتفرم' },
            ].map((s) => (
              <div key={s.label} className={styles.hero__stat}>
                <div className={styles['hero__stat-value']}>{s.value}</div>
                <div className={styles['hero__stat-label']}>{s.label}</div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Marquee */}
      <div className={styles['marquee-section']} aria-hidden="true">
        <div className={styles['marquee-track']}>
          {[...marqueeItems, ...marqueeItems].map((item, i) => (
            <span key={i} className={styles['marquee-item']}>{item}</span>
          ))}
        </div>
      </div>

      {/* Features */}
      <section id="features" className={styles.features}>
        <div className="container">
          <div className={styles.features__header}>
            <p className={styles.features__eyebrow}>ویژگی‌های پلتفرم</p>
            <h2 className={styles.features__title}>هر آنچه برای معامله مواد اولیه در مقیاس بزرگ نیاز دارید</h2>
          </div>
          <div className={styles.features__grid}>
            {features.map((f) => (
              <div key={f.title} className={styles['feature-card']}>
                <div className={`${styles['feature-card__icon']} ${styles[`feature-card__icon--${f.class}`]}`}>{f.icon}</div>
                <h3 className={styles['feature-card__title']}>{f.title}</h3>
                <p className={styles['feature-card__desc']}>{f.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* CTA */}
      <section className={styles['cta-section']}>
        <div className="container">
          <h2 className={styles['cta-section__title']}>
            آماده تحول در معاملات<br />
            <span className={styles.gradient}>مواد اولیه خود هستید؟</span>
          </h2>
          <p className={styles['cta-section__subtitle']}>
            به بیش از ۱۲٬۰۰۰ شرکت که اکنون با هوشمندتر در رانکس معامله می‌کنند بپیوندید. بدون قرارداد و بدون هزینه راه‌اندازی.
          </p>
          <div className={styles['cta-section__actions']}>
            <Link href="/auth/register" style={{ display: 'inline-flex', alignItems: 'center', gap: '8px', padding: '16px 40px', borderRadius: '16px', fontFamily: 'inherit', fontSize: '16px', fontWeight: 700, color: '#fff', background: 'var(--color-brand-primary)', textDecoration: 'none', boxShadow: '0 8px 32px rgba(10,132,255,0.40)' }}>
              شروع کنید — رایگان است <ArrowRight size={18} />
            </Link>
          </div>
        </div>
      </section>

      {/* Footer */}
      <footer className={styles.footer}>
        <div className="container">
          <div className={styles.footer__grid}>
            <div className={styles.footer__brand}>
              <Link href="/" className={styles.navbar__logo} style={{ textDecoration: 'none' }}>
                <div className={styles['navbar__logo-icon']}>R</div>
                <span className={styles.navbar__brand}>Raw<span>nex</span></span>
              </Link>
              <p>پلتفرم معاملات مواد اولیه سازمانی مبتنی بر هوش مصنوعی، طراحی شده برای بازارهای جهانی.</p>
            </div>
            {[
              { title: 'پلتفرم', links: ['بازار', 'سیستم RFQ', 'مزایده', 'تحلیل', 'لجستیک'] },
              { title: 'شرکت',  links: ['درباره ما', 'فرصت‌های شغلی', 'وبلاگ', 'رسانه', 'همکاران'] },
              { title: 'قانونی',    links: ['حریم خصوصی', 'شرایط استفاده', 'سیاست کوکی', 'انطباق'] },
            ].map((col) => (
              <div key={col.title}>
                <p className={styles['footer__col-title']}>{col.title}</p>
                <div className={styles.footer__links}>
                  {col.links.map((link) => (
                    <a key={link} href="#" className={styles.footer__link}>{link}</a>
                  ))}
                </div>
              </div>
            ))}
          </div>
          <div className={styles.footer__bottom}>
            <p className={styles.footer__copy}>© ۲۰۲۶ رانکس تکنولوژیز. تمامی حقوق محفوظ است.</p>
            <p className={styles.footer__copy}>ساخته شده برای جامعه معامله‌گران جهانی</p>
          </div>
        </div>
      </footer>
    </>
  );
}
