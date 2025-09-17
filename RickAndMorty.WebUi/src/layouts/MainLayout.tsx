// import { useTranslation } from 'react-i18next';
import { Outlet } from 'react-router';

export default function MainLayout() {
    // const { t, i18n } = useTranslation();

    return (
        <>
            <header />
            <main>
                <Outlet />
            </main>
        </>
    )
}
