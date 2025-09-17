import { Card, CardContent, CardMedia, Typography } from "@mui/material";
import type { Character } from "../../models/character";
import { useTranslation } from "react-i18next";

type Props = {
    character: Character;
}

export default function CharacterCard({ character }: Props) {
    const { t } = useTranslation();

    return (
        <Card className="character-card">
            <CardMedia
                component="img"
                alt={character.name ?? ""}
                height="140"
                image={character.image ?? ""}
            />
            <CardContent>
                <Typography variant="h6" component="div">
                    {character.name}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                    {character.species} - {character.gender}
                </Typography>
                <Typography variant="caption" color="text.secondary">
                    {t('labels.origin')}: {character.origin?.name ?? t('labels.unknown')}
                </Typography>            
            </CardContent>
        </Card>
    );
}