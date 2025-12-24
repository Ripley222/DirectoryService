import Link from "next/link";
import "./globals.css";

export default function RootLayout({ children } : { children: React.ReactNode }) {
    return (
        <html lang="en">
            <body className="flex min-h-screen">
                {/* Sidebar */}
                <aside className="w-64 bg-red-300 text-white p-4">
                    <h1 className="text-2xl font-bold mb-6">Меню</h1>
                    <nav className="flex flex-col gap-2">
                        <Link href="/" className="hover:bg-gray-700 p-2 rounded">Главная</Link>
                        <Link href="/locations" className="hover:bg-gray-700 p-2 rounded">Локации</Link>
                        <Link href="/departments" className="hover:bg-gray-700 p-2 rounded">Подразделения</Link>
                        <Link href="/positions" className="hover:bg-gray-700 p-2 rounded">Позиции</Link>
                    </nav>
                </aside>

                {/* Main content */}
                <div className="flex-1">
                    {/* Header */}
                    <header className="bg-gray-100 p-4 shadow">
                        <h2 className="text-2xl font-semibold">Сервис подразделений</h2>
                    </header>

                    <main className="p-6">
                        {children}
                    </main>
                </div>
            </body>
        </html>
    )
}