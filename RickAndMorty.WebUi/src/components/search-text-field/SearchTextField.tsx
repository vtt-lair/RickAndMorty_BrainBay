import { FormControl, InputAdornment, TextField } from "@mui/material";
import SearchIcon from '@mui/icons-material/Search';
import ClearIcon from '@mui/icons-material/Clear';
import { useState } from "react";

type Props = {
    onSearch: (query: string) => void;
    onClear: () => void;
}

export default function SearchTextField({ onSearch, onClear }: Props) {
    const [searchValue, setSearchValue] = useState("");
    const [showClearIcon, setShowClearIcon] = useState("none");

    const handleChange = (event: React.ChangeEvent<HTMLInputElement>): void => {
        const value = event.target.value;
        setSearchValue(value);
        setShowClearIcon(value === "" ? "none" : "flex");
        onSearch(value);
    };

    const handleClearClick = (): void => {
        setSearchValue("");
        setShowClearIcon("none");
        onClear();        
    };

    return (
        <FormControl>
            <TextField
                size="small"
                variant="outlined"
                value={searchValue}
                onChange={handleChange}
                slotProps={{
                    input: {
                        startAdornment: (
                            <InputAdornment position="start">
                                <SearchIcon />
                            </InputAdornment>
                        ),
                        endAdornment: (
                            <InputAdornment
                                position="end"
                                sx={{ display: showClearIcon }}
                                onClick={handleClearClick}
                            >
                                <ClearIcon className="clear-icon" />
                            </InputAdornment>
                        )
                    }
                }}
            />
        </FormControl>
    );
}