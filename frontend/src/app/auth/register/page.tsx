'use client';
import Link from 'next/link';
import { useState } from 'react';
import { Mail, Lock, Eye, EyeOff, User, Building2, Phone, ArrowRight, CheckCircle } from 'lucide-react';
import styles from '../auth.module.scss';

const steps = [
  { num: 1, label: 'حساب' },
  { num: 2, label: 'شرکت' },
  { num: 3, label: 'تأیید' },
];

const companyTypes = ['سازنده', 'توزیع‌کننده', 'معامله‌گر / کارگزار', 'تولیدکننده / معدنچی', 'پالایشگر', 'سایر'];

export default function RegisterPage() {
  const [currentStep, setCurrentStep] = useState(1);
  const [showPass, setShowPass] = useState(false);
  const [role, setRole] = useState<string | null>(null);

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
            شروع به معامله<br />
            <span className={styles.gradient}>در دقایق</span>
          </h1>

          <p className={styles['auth__panel-subtitle']}>
            حساب رایگان خود را بسازید و به بزرگ‌ترین بازار B2B مواد اولیه جهان دسترسی پیدا کنید. KYC در کمتر از ۲۴ ساعت.
          </p>

          <div style={{ display: 'flex', flexDirection: 'column', gap: '12px', marginBottom: '32px' }}>
            {[
              { icon: '✅', text: 'رایگان همیشه — بدون اطلاعات کارت اعتباری' },
              { icon: '⚡', text: 'تطابق هوشمند بلادرنگ' },
              { icon: '🔒', text: 'امانت‌داری بانکی برای هر سفارش' },
              { icon: '🌍', text: 'معامله جهانی با بیش از ۱۸۰ کشور' },
              { icon: '🛡️', text: 'تأیید KYC/KYB از طریق Sumsub' },
            ].map((item) => (
              <div key={item.text} style={{ display: 'flex', gap: '12px', alignItems: 'center' }}>
                <span style={{ fontSize: '16px' }}>{item.icon}</span>
                <span style={{ fontSize: '13px', color: 'rgba(255,255,255,0.70)' }}>{item.text}</span>
              </div>
            ))}
          </div>

          <div className={styles['auth__testimonial']}>
            <p className={styles['auth__testimonial-text']}>
              طی یک روز وارد شدیم. الان HDPE را از ۱۲ کشور با پوشش کامل امانت‌داری تهیه می‌کنیم.
            </p>
            <div className={styles['auth__testimonial-author']}>
              <div className={styles['auth__testimonial-avatar']}>س.ا</div>
              <div>
                <p className={styles['auth__testimonial-name']}>سارا احمدی</p>
                <p className={styles['auth__testimonial-company']}>مدیرعامل · گروه پارس پلیمر</p>
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

          {/* Steps */}
          <div className={styles.steps}>
            {steps.map((s, i) => (
              <div key={s.num} className={styles.step} style={{ flex: i < steps.length - 1 ? 1 : 'unset' }}>
                <div className={`${styles['step__num']} ${currentStep === s.num ? styles['step__num--active'] : ''} ${currentStep > s.num ? styles['step__num--done'] : ''}`}>
                  {currentStep > s.num ? '✓' : s.num}
                </div>
                <span className={`${styles['step__label']} ${currentStep === s.num ? styles['step__label--active'] : ''}`}>{s.label}</span>
                {i < steps.length - 1 && <div className={styles['step__line']} />}
              </div>
            ))}
          </div>

          {currentStep === 1 && (
            <>
              <h2 className={styles['auth__title']}>ایجاد حساب</h2>
              <p className={styles['auth__subtitle']}>
                قبلاً حساب دارید؟ <Link href="/auth/login">ورود ←</Link>
              </p>

              <div className={styles['social-btns']} style={{ marginTop: '24px' }}>
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
                <div className={styles['form__row']}>
                  <div className={styles['form__group']}>
                    <label className={styles['form__label']}>نام</label>
                    <div className={styles['form__input-wrap']}>
                      <span className={styles['form__input-icon']}><User size={15} /></span>
                      <input type="text" placeholder="علی" className={`${styles['form__input']} ${styles['form__input--with-icon']}`} />
                    </div>
                  </div>
                  <div className={styles['form__group']}>
                    <label className={styles['form__label']}>نام خانوادگی</label>
                    <div className={styles['form__input-wrap']}>
                      <input type="text" placeholder="کریمی" className={styles['form__input']} />
                    </div>
                  </div>
                </div>

                <div className={styles['form__group']}>
                  <label className={styles['form__label']}>ایمیل کاری</label>
                  <div className={styles['form__input-wrap']}>
                    <span className={styles['form__input-icon']}><Mail size={15} /></span>
                    <input type="email" placeholder="you@company.com" className={`${styles['form__input']} ${styles['form__input--with-icon']}`} />
                  </div>
                </div>

                <div className={styles['form__group']}>
                  <label className={styles['form__label']}>رمز عبور</label>
                  <div className={styles['form__input-wrap']}>
                    <span className={styles['form__input-icon']}><Lock size={15} /></span>
                    <input type={showPass ? 'text' : 'password'} placeholder="حداقل ۸ کاراکتر" className={`${styles['form__input']} ${styles['form__input--with-icon']}`} />
                    <button type="button" className={styles['form__input-action']} onClick={() => setShowPass(v => !v)}>
                      {showPass ? <EyeOff size={15} /> : <Eye size={15} />}
                    </button>
                  </div>
                </div>

                <div className={styles['form__group']}>
                  <label className={styles['form__label']}>نقش شما</label>
                  <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '8px' }}>
                    {['خریدار', 'فروشنده', 'معامله‌گر', 'کارگزار'].map((r) => (
                      <button key={r} type="button" onClick={() => setRole(r)} style={{ padding: '10px', borderRadius: '10px', fontSize: '13px', fontWeight: 600, fontFamily: 'inherit', cursor: 'pointer', border: `1.5px solid ${role === r ? 'var(--color-brand-primary)' : 'var(--border-default)'}`, background: role === r ? 'var(--bg-active)' : 'var(--bg-card)', color: role === r ? 'var(--color-brand-primary)' : 'var(--text-secondary)', transition: 'all 200ms ease' }}>
                        {r}
                      </button>
                    ))}
                  </div>
                </div>

                <button type="button" className={styles['form__submit']} onClick={() => setCurrentStep(2)}>
                  Continue <ArrowRight size={16} style={{ display: 'inline', verticalAlign: 'middle', marginLeft: '4px' }} />
                </button>

                <p className={styles['form__terms']}>
                  با ساختن حساب، با <Link href="/legal/terms">شرایط</Link> و <Link href="/legal/privacy">حریم خصوصی</Link> موافقت می‌کنید.
                </p>
              </form>
            </>
          )}

          {currentStep === 2 && (
            <>
              <h2 className={styles['auth__title']}>شرکت شما</h2>
              <p className={styles['auth__subtitle']}>درباره سازمانتان بگویید</p>

              <form className={styles.form} style={{ marginTop: '24px' }} onSubmit={(e) => e.preventDefault()}>
                <div className={styles['form__group']}>
                  <label className={styles['form__label']}>نام شرکت</label>
                  <div className={styles['form__input-wrap']}>
                    <span className={styles['form__input-icon']}><Building2 size={15} /></span>
                    <input type="text" placeholder="پارس فولاد" className={`${styles['form__input']} ${styles['form__input--with-icon']}`} />
                  </div>
                </div>

                <div className={styles['form__group']}>
                  <label className={styles['form__label']}>نوع شرکت</label>
                  <select className={styles['form__input']} style={{ height: '46px', cursor: 'pointer' }}>
                    <option value="">انتخاب نوع…</option>
                    {companyTypes.map((t) => <option key={t}>{t}</option>)}
                  </select>
                </div>

                <div className={styles['form__row']}>
                  <div className={styles['form__group']}>
                    <label className={styles['form__label']}>کشور</label>
                    <select className={styles['form__input']} style={{ height: '46px', cursor: 'pointer' }}>
                      <option>ایران</option>
                      <option>امارات</option>
                      <option>ترکیه</option>
                      <option>آلمان</option>
                      <option>چین</option>
                    </select>
                  </div>
                  <div className={styles['form__group']}>
                    <label className={styles['form__label']}>تلفن</label>
                    <div className={styles['form__input-wrap']}>
                      <span className={styles['form__input-icon']}><Phone size={15} /></span>
                      <input type="tel" placeholder="+98 21 …" className={`${styles['form__input']} ${styles['form__input--with-icon']}`} />
                    </div>
                  </div>
                </div>

                <div className={styles['form__group']}>
                  <label className={styles['form__label']}>شماره ثبت شرکت</label>
                  <input type="text" placeholder="شماره ثبت #" className={styles['form__input']} />
                </div>

                <div style={{ display: 'flex', gap: '12px', marginTop: '8px' }}>
                  <button type="button" onClick={() => setCurrentStep(1)} style={{ flex: 1, height: '48px', background: 'var(--bg-input)', border: '1px solid var(--border-default)', borderRadius: '14px', fontFamily: 'inherit', fontSize: '14px', fontWeight: 600, color: 'var(--text-secondary)', cursor: 'pointer' }}>
                    بازگشت
                  </button>
                  <button type="button" className={styles['form__submit']} style={{ flex: 2, margin: 0 }} onClick={() => setCurrentStep(3)}>
                    Continue <ArrowRight size={16} style={{ display: 'inline', verticalAlign: 'middle', marginLeft: '4px' }} />
                  </button>
                </div>
              </form>
            </>
          )}

          {currentStep === 3 && (
            <>
              <h2 className={styles['auth__title']}>تأیید ایمیل</h2>
              <p className={styles['auth__subtitle']}>کد ۶ رقمی به ایمیل شما ارسال شد</p>

              <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '32px', marginTop: '32px' }}>
                <div style={{ fontSize: '64px' }}>📧</div>
                <div style={{ display: 'flex', gap: '12px' }}>
                  {[...Array(6)].map((_, i) => (
                    <input key={i} type="text" maxLength={1} style={{ width: '48px', height: '56px', textAlign: 'center', fontSize: '20px', fontWeight: 700, fontFamily: 'inherit', background: 'var(--bg-card)', border: '1.5px solid var(--border-default)', borderRadius: '12px', color: 'var(--text-primary)', outline: 'none', transition: 'border-color 200ms ease' }} />
                  ))}
                </div>
                <p style={{ fontSize: '13px', color: 'var(--text-secondary)', textAlign: 'center' }}>
                  دریافت نکردید؟ <button style={{ background: 'none', border: 'none', color: 'var(--text-brand)', fontFamily: 'inherit', fontSize: '13px', fontWeight: 600, cursor: 'pointer' }}>ارسال مجدد کد</button>
                </p>

                <Link href="/dashboard" style={{ display: 'flex', width: '100%', alignItems: 'center', justifyContent: 'center', gap: '8px', height: '48px', background: 'var(--color-brand-primary)', color: '#fff', fontFamily: 'inherit', fontSize: '15px', fontWeight: 600, borderRadius: '14px', textDecoration: 'none', boxShadow: '0 4px 20px rgba(10,132,255,0.35)' }}>
                  <CheckCircle size={18} /> تکمیل ثبت‌نام
                </Link>
              </div>
            </>
          )}
        </div>
      </div>
    </div>
  );
}
