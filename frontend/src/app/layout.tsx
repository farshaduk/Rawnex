import type { Metadata, Viewport } from "next";
import "@/styles/globals.scss";

export const metadata: Metadata = {
  title: "رانکس — پلتفرم معاملات مواد اولیه",
  description: "پلتفرم سازمانی B2B برای معامله مواد اولیه. خریداران، فروشندگان، معامله‌گران و کارگزاران را در سراسر جهان متصل می‌کند.",
  keywords: ["مواد اولیه", "معاملات B2B", "کالاها", "بازار", "تدارکات"],
};

export const viewport: Viewport = {
  width: "device-width",
  initialScale: 1,
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="fa" dir="rtl" suppressHydrationWarning>
      <head>
        <link rel="preconnect" href="https://fonts.googleapis.com" />
        <link rel="preconnect" href="https://fonts.gstatic.com" crossOrigin="anonymous" />
        <link href="https://fonts.googleapis.com/css2?family=Vazirmatn:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
      </head>
      <body>{children}</body>
    </html>
  );
}
