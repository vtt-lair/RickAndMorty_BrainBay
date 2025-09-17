import { useEffect } from "react";
import { useTranslation } from "react-i18next";

type Props = {
    title: string,
    children: any,
}

export default function Page({ title, children }: Props) {
    const { t } = useTranslation();

    useEffect(() => {
        document.title = title ? `${t('general.site_name')} - ${t(title)}` : `${t('general.site_name')}`;
    }, [t, title]);

    return children;
}