'use client';
import Link from 'next/link';
import { useState } from 'react';
import { Mail, Lock, Eye, EyeOff, ArrowRight } from 'lucide-react';
import styles from '../auth.module.scss';

export default function LoginPage() {
  const [showPass, setShowPass] = useState(false);

  return (
    <div className={styles.auth}>
      {/* ── Left Panel ── */}
      <div className={styles['auth__panel']}>
        <div className={styles['auth__panel-grid']} aria-hidden="true" />
        <div className={styles['auth__panel-content']}>
          <Link href="/" className={styles['auth__panel-logo']}>
            <div className={styles['auth__panel-logo-icon']}>R</div>
            <span className={styles['auth__panel-brand']}>Raw<span>nex</span></span>
          </Link>

          <h1 className={styles['auth__panel-title']}>
            آینده معاملات<br />
            <span className={styles.gradient}>B2B مواد اولیه</span>
          </h1>

          <p className={styles['auth__panel-subtitle']}>
            به بیش از ۱۲٬۰۰۰ شرکت که فلزات، مواد شیمیایی، پلیمرها
            و کالاهای کشاورزی را در امن‌ترین پلتفرم جهان معامله می‌کنند بپیوندید.
          </p>

          <div className={styles['auth__stats']}>
            {[
              { value: '$4.8B+', label: 'حجم سالانه' },
              { value: '180+',   label: 'کشور' },
              { value: '99.97%', label: 'آپ‌تایم' },
              { value: '< 24h',  label: 'تأیید KYC' },
            ].map((s) => (
              <div key={s.label} className={styles['auth__stat']}>
                <div className={styles['auth__stat-value']}>{s.value}</div>
                <div className={styles['auth__stat-label']}>{s.label}</div>
              </div>
            ))}
          </div>

          <div className={styles['auth__testimonial']}>
            <p className={styles['auth__testimonial-text']}>
              رانکس نحوه تأمین مس ما را متحول کرد. تطابق هوشمند ۴۰٪ زمان تدارکات ما را صرفه‌جویی کرد.
            </p>
            <div className={styles['auth__testimonial-author']}>
              <div className={styles['auth__testimonial-avatar']}>م.ر</div>
              <div>
                <p className={styles['auth__testimonial-name']}>محمد رضایی</p>
                <p className={styles['auth__testimonial-company']}>رئیس تدارکات · فولاد یزد</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* ── Right Panel (Form) ── */}
      <div className={styles['auth__form-panel']}>
        <div className={styles['auth__form-container']}>
          <Link href="/" className={styles['auth__form-logo']}>
            <div className={styles['auth__form-logo-icon']}>R</div>
            <span className={styles['auth__form-logo-brand']}>Raw<span>nex</span></span>
          </Link>

          <h2 className={styles['auth__title']}>خوش برگشتید</h2>
          <p className={styles['auth__subtitle']}>
            حساب ندارید؟ <Link href="/auth/register">رایگان بسازید ←</Link>
          </p>

          {/* Social */}
          <div className={styles['social-btns']}>
            <button className={styles['social-btn']}>
              <span style={{ fontSize: '18px' }}>G</span> گوگل
            </button>
            <button className={styles['social-btn']}>
              <span style={{ fontSize: '18px' }}>in</span> لینکداین
            </button>
          </div>

          <div className={styles['auth__divider']}>
            <span>یا با ایمیل ادامه دهید</span>
          </div>

          <form className={styles.form} onSubmit={(e) => e.preventDefault()}>
            <div className={styles['form__group']}>
              <label className={styles['form__label']} htmlFor="email">آدرس ایمیل</label>
              <div className={styles['form__input-wrap']}>
                <span className={styles['form__input-icon']}><Mail size={16} /></span>
                <input
                  id="email"
                  type="email"
                  placeholder="you@company.com"
                  className={`${styles['form__input']} ${styles['form__input--with-icon']}`}
                  autoComplete="email"
                />
              </div>
            </div>

            <div className={styles['form__group']}>
              <div className={styles['form__row-between']}>
                <label className={styles['form__label']} htmlFor="password">رمز عبور</label>
                <Link href="/auth/forgot-password" className={styles['form__forgot']}>فراموشی رمز عبور؟</Link>
              </div>
              <div className={styles['form__input-wrap']}>
                <span className={styles['form__input-icon']}><Lock size={16} /></span>
                <input
                  id="password"
                  type={showPass ? 'text' : 'password'}
                  placeholder="••••••••"
                  className={`${styles['form__input']} ${styles['form__input--with-icon']}`}
                  autoComplete="current-password"
                />
                <button
                  type="button"
                  className={styles['form__input-action']}
                  onClick={() => setShowPass(v => !v)}
                  aria-label={showPass ? 'Hide password' : 'Show password'}
                >
                  {showPass ? <EyeOff size={16} /> : <Eye size={16} />}
                </button>
              </div>
            </div>

            <div className={styles['form__checkbox-group']}>
              <input type="checkbox" id="remember" className={styles['form__checkbox']} />
              <label htmlFor="remember" className={styles['form__checkbox-label']}>
                ۳۰ روز به خاطر بسپار
              </label>
            </div>

            <Link href="/dashboard" style={{ display: 'flex', width: '100%', alignItems: 'center', justifyContent: 'center', gap: '8px', height: '48px', background: 'var(--color-brand-primary)', color: '#fff', fontFamily: 'inherit', fontSize: '15px', fontWeight: 600, borderRadius: '14px', textDecoration: 'none', boxShadow: '0 4px 20px rgba(10,132,255,0.35)', marginTop: '8px' }}>
              ورود <ArrowRight size={16} />
            </Link>

            <p className={styles['form__terms']}>
              محافظت‌شده با رمزنگاری SSL 256‌بیتی. اطلاعات شما نزد ما امن است.
            </p>
          </form>
        </div>
      </div>
    </div>
  );
}
