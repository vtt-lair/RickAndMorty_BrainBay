import { Autocomplete, Box, Button, CardActions, CardContent, FormControl, Grid, InputLabel, MenuItem, Select, Stack, TextField, Typography } from "@mui/material"
import { useTranslation } from "react-i18next";
import * as yup from 'yup';
import { useFormik } from "formik";
import queryClient from "../../utils/query-client";
import api from "../../services/api";
import { useLoaderData, useNavigate } from "react-router";
import { Character } from "../../models/character";
import SnackbarUtils from "../snackbar-utils/SnackbarUtils";
import type { Planet } from "../../models/planet";

type AddCharacterFormProps = {
    name: string,
    species: string | null,
    type: string | null,
    gender: string | null,
    originId: number | null,
    locationId: number | null,
    image: string | null,
}

export async function loader() {
    let planets = await queryClient.ensureQueryData({
        queryKey: ['planets-list'],
        queryFn: () => api.planet.getPlanets(),
    });    

    return [... planets];
}


export default function CharacterForm() {
    const planets = useLoaderData() as Awaited<ReturnType<typeof loader>>;
    const { t } = useTranslation();
    const navigate = useNavigate();

    const validationSchema = yup.object({
        name: yup.string().required('Name is required'),
        image: yup
            .string()
            .url('Image must be a valid URL')
            .matches(
                /\.(jpg|jpeg|png|gif|bmp|webp|svg)$/i,
                'Image must be a valid image file (jpg, jpeg, png, gif, bmp, webp, svg)'
            )
            .optional(),
    });

    const initialValues: AddCharacterFormProps = {
        name: '',
        species: '',
        type: '',
        gender: '',
        originId: 0,
        locationId: 0,
        image: '',
    }

    const formik = useFormik({
        initialValues,
        validationSchema,
        onSubmit: (values) => submitCharacter(values),
    });

    async function submitCharacter(values: AddCharacterFormProps) {
        if (!formik.isValid) {
            return;
        }

        const character = new Character(
            0,
            null,
            values.name,
            values.species,
            values.type,
            values.gender,
            values.originId,
            null,
            values.locationId,
            null,
            values.image
        )

        const saved = await api.character.saveCharacter(character);
        if (saved) {
            SnackbarUtils.success(t('messages.character_saved_success'));
            navigate('/character-list');
        } else {
            SnackbarUtils.error(t('messages.character_saved_error'));
        }
    }
    
    return (
        <Box 
            sx={{
                width: '100%',
                minHeight: '100vh',
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
                py: 2
            }}
            className="character-card-box"
        >
            <Box
                sx={{
                    width: '100%',
                    maxWidth: { xs: '95%', sm: '600px', md: '800px', lg: '900px' },
                    px: { xs: 2, sm: 3, md: 4 }
                }}
            >            
            <Stack direction={"row"} spacing={2} mb={2} justifyContent="space-between" alignItems="start">
                <Typography sx={{ typography: { md: 'h3', sm: 'h4', xs: 'h6' } }} component="div" className="page-title">
                    {t('titles.add_character')}
                </Typography>
                <Button variant="outlined" color="secondary" onClick={() => navigate(-1)}>
                    {t('buttons.back')}
                </Button>
            </Stack>

            <form onSubmit={formik.handleSubmit} key="addCharacterForm">
                <Box>
                    <Grid container spacing={2}>
                    <Grid size={{ xs: 12 }}>
                        <TextField
                            id="name"
                            fullWidth
                            variant="outlined"
                            label={t('labels.character_name')}
                            value={formik.values.name}
                            onChange={formik.handleChange}
                            onBlur={formik.handleBlur}
                            helperText={formik.touched.name && formik.errors.name}
                            error={formik.touched.name && Boolean(formik.errors.name)}
                        />
                    </Grid>

                    <Grid size={{ xs: 12, md: 6 }}>
                        <TextField
                            id="species"
                            fullWidth
                            variant="outlined"
                            label={t('labels.character_species')}
                            value={formik.values.species}
                            onChange={formik.handleChange}
                            onBlur={formik.handleBlur}
                            helperText={formik.touched.species && formik.errors.species}
                            error={formik.touched.species && Boolean(formik.errors.species)}
                        />
                    </Grid>
                    <Grid size={{ xs: 12, md: 6 }}>
                        <TextField
                            id="type"
                            fullWidth
                            variant="outlined"
                            label={t('labels.character_type')}
                            value={formik.values.type}
                            onChange={formik.handleChange}
                            onBlur={formik.handleBlur}
                            helperText={formik.touched.type && formik.errors.type}
                            error={formik.touched.type && Boolean(formik.errors.type)}
                        />
                    </Grid>

                    <Grid size={{ xs: 12, md: 6 }}>
                        <TextField
                            id="gender"
                            fullWidth
                            variant="outlined"
                            label={t('labels.character_gender')}
                            value={formik.values.gender}
                            onChange={formik.handleChange}
                            onBlur={formik.handleBlur}
                            helperText={formik.touched.gender && formik.errors.gender}
                            error={formik.touched.gender && Boolean(formik.errors.gender)}
                        />
                    </Grid>
                    <Grid size={{ xs: 12, md: 6 }}>
                        <Autocomplete
                            fullWidth                            
                            options={planets}
                            groupBy={(planet: Planet) => t(planet.dimension ?? "")}
                            getOptionLabel={(planet: Planet) => planet.name ?? ""}
                            value={planets.find(planet => planet.id === formik.values.locationId) || null}
                            onChange={(_, value) => { 
                                formik.setFieldValue('locationId', value?.id || 0) 
                            }}
                            onBlur={() => formik.setFieldTouched('locationId', true)}
                            isOptionEqualToValue={(option, value) => option.id === value.id}
                            renderInput={(params) => <TextField 
                                {...params} 
                                label={t('labels.character_location')}
                                helperText={formik.touched.locationId && formik.errors.locationId}
                                error={formik.touched.locationId && Boolean(formik.errors.locationId)}
                            />}                            
                        />                          
                    </Grid>

                    <Grid size={{ xs: 12, md: 6 }}>
                        <Autocomplete
                            fullWidth                            
                            options={planets}
                            groupBy={(planet: Planet) => t(planet.dimension ?? "")}
                            getOptionLabel={(planet: Planet) => planet.name ?? ""}
                            value={planets.find(planet => planet.id === formik.values.locationId) || null}
                            onChange={(_, value) => { 
                                formik.setFieldValue('originId', value?.id || 0) 
                            }}
                            onBlur={() => formik.setFieldTouched('originId', true)}
                            isOptionEqualToValue={(option, value) => option.id === value.id}
                            renderInput={(params) => <TextField 
                                {...params} 
                                label={t('labels.character_origin')}
                                helperText={formik.touched.originId && formik.errors.originId}
                                error={formik.touched.originId && Boolean(formik.errors.originId)}
                            />}                            
                        />
                    </Grid>
                    <Grid size={{ xs: 12, md: 6 }}>
                        <TextField
                            id="image"
                            fullWidth
                            variant="outlined"
                            label={t('labels.character_image')}
                            value={formik.values.image}
                            onChange={formik.handleChange}
                            onBlur={formik.handleBlur}
                            helperText={formik.touched.image && formik.errors.image}
                            error={formik.touched.image && Boolean(formik.errors.image)}
                        />
                    </Grid>
                </Grid>

                <CardActions>
                    <Stack direction="row" spacing={2} justifyContent="flex-start" mt={2}>
                        <Button type="button" variant="outlined" color="secondary" onClick={() => formik.resetForm()}>
                            {t('buttons.reset')}
                        </Button>
                        <Button type="submit"  variant="contained" color="primary" disabled={!formik.isValid || formik.isSubmitting}>
                            {t('buttons.save')}
                        </Button>
                    </Stack>
                </CardActions>
                </Box>
            </form>
            </Box>
        </Box>
    )
}