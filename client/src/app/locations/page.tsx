"use client"

import Link from "next/link";
import {useState} from "react";
import {locationsApi} from "@/entities/locations/api";
import {useQuery} from "@tanstack/react-query";

const PAGE_SIZE = 20;

export default function LocationsPage() {
    const [page, setPage] = useState(1);

    const {data: locations, isPending, error} = useQuery({
        queryFn: () => locationsApi.getLocations({page: page, pageSize: PAGE_SIZE}),
        queryKey: ["locations"]
    });

    if (isPending) {
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>Ошибка: {error.message}</div>;
    }

    return (
        <div className="p-6">
            <div>
                <h1 className="text-2xl font-bold mb-4">Локации</h1>
                <p>Список всех локаций где расположены подразделения</p>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {locations?.map((location) => (
                    <Link
                        key={location.locationId}
                        href={`/locations/${location.locationId}`}
                        className="rounded-xl border border-gray-200 bg-white p-5 shadow-sm hover:shadow-md transition">

                        <h2 className="text-lg font-semibold mb-2">
                            {location.locationName}
                        </h2>

                        <p className="text-sm text-gray-600 mb-1">
                            {location.locationAddress}
                        </p>

                        <p className="text-sm text-gray-500">
                            {location.locationTimeZone}
                        </p>

                        <p className="text-sm text-gray-500">
                            {location.isActive ? "Локация активна" : "Локация удалена"}
                        </p>
                    </Link>
                ))}
            </div>
        </div>
    );
}