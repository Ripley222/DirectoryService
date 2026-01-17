import "./globals.css"
import Layout from "@/app/app-layout";
import type {Metadata} from "next";
import React from "react";

export const metadata: Metadata = {
    title: "Directory Service",
    description: "Сервис управления локациями, позициями и подразделениями",
};

export default function RootLayout({children}: {
    children: React.ReactNode
}) {
    return (
        <html lang="en">
            <body>
                <Layout>{children}</Layout>
            </body>
        </html>
    )
}