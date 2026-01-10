import "./globals.css"
import Layout from "@/features/layout/app-layout";

export default function RootLayout({children}: { children: React.ReactNode }) {
    return (
        <html lang="en">
        <body>
        <Layout>{children}</Layout>
        </body>
        </html>
    )
}