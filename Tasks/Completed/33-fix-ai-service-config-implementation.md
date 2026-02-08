# Task 33: Fix AI Service Config Implementation

## Objective
Fix the AI Service Config implementation to follow project standards for typed DTOs, add a fallback model field to the table, and enhance the frontend to auto-populate available models based on the selected provider and API key.

## Background
The current implementation violates our service standards by returning `object` instead of strongly typed DTOs. Additionally, the frontend uses a simple text input for model selection instead of fetching and displaying available models from the provider. We also need to add a fallback model option to the configuration.

## Requirements

### 1. Fix Service to Use Typed DTOs (Violates `02_How_We_Do_services.md`)

**Current Issue**: `AiConfigService.cs` returns `object` from all service methods.

**Required Changes**:
- Update all service methods in `AIInterviewer.ServiceInterface\Services\Configuration\AiConfigService.cs` to return strongly typed DTOs instead of `object`:
  - `Any(ListAiConfigs request)` → Return `ListAiConfigsResponse`
  - `Any(GetAiConfig request)` → Return `AiConfigResponse`
  - `Any(CreateAiConfig request)` → Return `AiConfigResponse`
  - `Any(UpdateAiConfig request)` → Return `AiConfigResponse`
  - `Any(DeleteAiConfig request)` → Can remain `void`

**Reference**: See `02_How_We_Do_services.md` line 5:
> We want to create strongly typed DTOs for any service and never return `object`.

### 2. Add Fallback Model Field to AiServiceConfig Table

**Required Changes**:

#### A. Update ServiceModel POCO
- **File**: `AIInterviewer.ServiceModel\Tables\Configuration\AiServiceConfig.cs`
- **Add Field**:
  ```csharp
  [StringLength(255)]
  public string? FallbackModelId { get; set; }
  ```

#### B. Create New Migration
- **File**: `AIInterviewer\Migrations\Migration1005_AddFallbackModelToAiServiceConfig.cs`
- **Action**: Add the `FallbackModelId` column to the `ai_service_config` table
- **Implementation**: Follow the "Dual Definition" pattern from `01_How_We_Do_Tables.md`
  - Define the table POCO inside the migration class with the `[Alias("ai_service_config")]` attribute
  - Include all existing fields plus the new `FallbackModelId` field
  - Use `Db.AddColumn<AiServiceConfig>(x => x.FallbackModelId)` in the `Up()` method
  - Use `Db.DropColumn<AiServiceConfig>(nameof(AiServiceConfig.FallbackModelId))` in the `Down()` method

#### C. Update DTOs
- **Files**: 
  - `AIInterviewer.ServiceModel\Types\Configuration\AiConfigResponse.cs`
  - `AIInterviewer.ServiceModel\Types\Configuration\CreateAiConfigRequest.cs`
  - `AIInterviewer.ServiceModel\Types\Configuration\UpdateAiConfigRequest.cs`
- **Add Field**: `public string? FallbackModelId { get; set; }`

#### D. Update Extension Methods
- **File**: `AIInterviewer.ServiceModel\Types\Configuration\ExtensionMethods\AiServiceConfigExtensions.cs`
- **Update Methods**: Ensure `ToTable()` and `ToDto()` methods handle the new `FallbackModelId` field

#### E. Run Migration
- Execute: `npm run migrate` from the `AIInterviewer/AIInterviewer` directory

#### F. Update TypeScript DTOs
- Execute: `.\Update-Dtos.ps1 -TaskNumber 33` from the repository root

### 3. Enhance Frontend Model Selection

**Current Issue**: The frontend uses a simple text input for `modelId`. Users must manually type model names.

**Required Enhancement**: Auto-populate available models when the user enters an API key.

#### A. Update Frontend Component
- **File**: `AIInterviewer.Client\src\pages\ai-configs.vue`

**Changes**:
1. Replace the `modelId` text input (line 89) with a dropdown/select element
2. Add a "Fetch Models" button or auto-fetch when API key is entered/changed
3. Add state management for:
   - `availableModels` (array of model names)
   - `loadingModels` (boolean for loading state)
   - `modelsFetched` (boolean to track if models have been loaded)
4. Implement `fetchModels()` function that:
   - Calls the appropriate service endpoint to list models based on `providerType` and `apiKey`
   - Populates the `availableModels` array
   - Handles errors gracefully (show error message if API key is invalid)
5. Update the model selection UI:
   - Show a dropdown with available models when `availableModels` is populated
   - Show a loading indicator when fetching models
   - Show a "Fetch Models" button if models haven't been fetched yet
   - Allow manual text input as a fallback if model fetching fails
6. Add a separate dropdown for `fallbackModelId` (optional field)

#### B. Backend Support (If Needed)
- Verify that the existing `GeminiModelsService` or equivalent can be called with an API key parameter
- If not, create a new service endpoint that accepts:
  - `providerType` (string: "Gemini" or "OpenAI")
  - `apiKey` (string)
  - Returns: List of available model IDs
- This service should use the `IAiProviderFactory` to get the appropriate provider and call `ListModelsAsync()`

**Note**: The service should NOT require a saved configuration to list models - it should accept the API key directly for validation/testing purposes.

## Checklist

### Backend Changes
- [ ] Update `AiConfigService.cs` to return typed DTOs instead of `object`
- [ ] Add `FallbackModelId` field to `AiServiceConfig.cs` (ServiceModel)
- [ ] Create `Migration1005_AddFallbackModelToAiServiceConfig.cs`
- [ ] Update `AiConfigResponse.cs` to include `FallbackModelId`
- [ ] Update `CreateAiConfigRequest.cs` to include `FallbackModelId`
- [ ] Update `UpdateAiConfigRequest.cs` to include `FallbackModelId`
- [ ] Update `AiServiceConfigExtensions.cs` to handle `FallbackModelId`
- [ ] Run `npm run migrate` to apply database changes
- [ ] Verify or create service endpoint for listing models with API key parameter
- [ ] Run `.\Update-Dtos.ps1 -TaskNumber 33` to update TypeScript DTOs

### Frontend Changes
- [ ] Replace `modelId` text input with dropdown in `ai-configs.vue`
- [ ] Add state management for `availableModels`, `loadingModels`, `modelsFetched`
- [ ] Implement `fetchModels()` function to retrieve models from backend
- [ ] Add "Fetch Models" button or auto-fetch trigger
- [ ] Add loading indicator for model fetching
- [ ] Add error handling for invalid API keys
- [ ] Add dropdown for `fallbackModelId` field
- [ ] Test model fetching for both Gemini and OpenAI providers
- [ ] Ensure fallback to manual text input if model fetching fails

### Testing
- [ ] Verify all service methods return correct typed DTOs
- [ ] Test creating AI config with fallback model
- [ ] Test updating AI config with fallback model
- [ ] Test model fetching with valid API keys (Gemini and OpenAI)
- [ ] Test model fetching with invalid API keys (should show error)
- [ ] Verify migration can be rolled back (`Down()` method works)
- [ ] Verify TypeScript DTOs are correctly generated

### Documentation
- [ ] Update clood files for Configuration domain if needed

## Standards References
- **Service Standards**: `Tasks/02_How_We_Do_services.md`
- **Table Standards**: `Tasks/01_How_We_Do_Tables.md`
- **Task Workflow**: `Tasks/00_How_we_do_tasks.md`

## Notes
- This task fixes violations of the strongly-typed DTO standard
- The fallback model provides a safety net if the primary model is unavailable
- Auto-populating models improves UX and reduces user errors from typos
