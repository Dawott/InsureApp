<template>
  <div>
    <div class="mb-6 flex justify-between items-center">
      <h1 class="text-2xl font-semibold text-gray-900">Typy ubezpieczenia</h1>
      <button @click="openCreateModal"
              class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md">
        Dodaj nowy
      </button>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="text-center py-4">
      <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
    </div>

    <!-- Error Message -->
    <div v-else-if="error" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
      {{ error }}
    </div>

    <!-- Types Table -->
    <div v-else class="bg-white shadow rounded-lg overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Nazwa</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Opis</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Akcje</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="type in insuranceTypes" :key="type.id" class="hover:bg-gray-50">
            <td class="px-6 py-4 whitespace-nowrap">
              <span class="text-sm font-medium text-gray-900">{{ type.name }}</span>
            </td>
            <td class="px-6 py-4">
              <span class="text-sm text-gray-500">{{ type.description || 'No description provided' }}</span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full"
                    :class="type.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'">
                {{ type.isActive ? 'Aktywny' : 'Nieaktywny' }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
              <div class="flex space-x-3">
                <button @click="editType(type)"
                        class="text-blue-600 hover:text-blue-900">
                  Edit
                </button>
                <button @click="confirmDelete(type)"
                        class="text-red-600 hover:text-red-900"
                        :disabled="hasReports(type)">
                  Delete
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Create/Edit Modal -->
    <div v-if="showModal" class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center">
      <div class="bg-white rounded-lg shadow-xl max-w-md w-full m-4">
        <div class="px-6 py-4 border-b border-gray-200">
          <div class="flex justify-between items-center">
            <h2 class="text-xl font-semibold text-gray-900">
              {{ selectedType ? 'Edytuj typ' : 'Stwórz typ' }}
            </h2>
            <button @click="closeModal" class="text-gray-400 hover:text-gray-500">
              <span class="sr-only">Close</span>
              <svg class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>
        </div>

        <form @submit.prevent="handleSubmit" class="p-6">
          <div class="space-y-4">
            <!-- Name -->
            <div>
              <label class="block text-sm font-medium text-gray-700">Nazwa</label>
              <input type="text"
                     v-model="formData.name"
                     required
                     class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500" />
            </div>

            <!-- Description -->
            <div>
              <label class="block text-sm font-medium text-gray-700">Opis</label>
              <textarea v-model="formData.description"
                        rows="3"
                        class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"></textarea>
            </div>

            <!-- Active Status -->
            <div class="flex items-center">
              <input type="checkbox"
                     v-model="formData.isActive"
                     class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded" />
              <label class="ml-2 block text-sm text-gray-900">Aktywny</label>
            </div>
          </div>

          <!-- Error Message -->
          <div v-if="modalError" class="mt-4 text-sm text-red-600">
            {{ modalError }}
          </div>

          <!-- Form Actions -->
          <div class="mt-6 flex justify-end space-x-3">
            <button type="button"
                    @click="closeModal"
                    class="bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50">
              Anuluj
            </button>
            <button type="submit"
                    :disabled="modalLoading"
                    class="bg-blue-600 py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50">
              {{ modalLoading ? 'Saving...' : 'Save' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Delete Confirmation Modal -->
    <div v-if="showDeleteModal" class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center">
      <div class="bg-white p-6 rounded-lg shadow-xl max-w-md w-full m-4">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Potwierdź usunięcie</h3>
        <p class="text-sm text-gray-500 mb-4">
          Czy na pewno chcesz skasować ten typ?
        </p>
        <div class="flex justify-end space-x-3">
          <button @click="showDeleteModal = false"
                  class="bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50">
            Anuluj
          </button>
          <button @click="deleteType"
                  class="bg-red-600 py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white hover:bg-red-700">
            Usuń
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import { insuranceTypesService } from '@/services/insuranceTypeService';

export default {
  name: 'InsuranceTypesList',
  setup() {
    const insuranceTypes = ref([]);
    const loading = ref(true);
    const error = ref(null);
    const showModal = ref(false);
    const showDeleteModal = ref(false);
    const selectedType = ref(null);
    const modalLoading = ref(false);
    const modalError = ref(null);

    const formData = ref({
      name: '',
      description: '',
      isActive: true
    });

    const loadTypes = async () => {
      try {
        loading.value = true;
        error.value = null;
        const response = await insuranceTypesService.getAllTypes();
        if (response.success) {
          insuranceTypes.value = response.data;
        } else {
          error.value = response.message;
        }
      } catch (err) {
        error.value = 'Błąd w trakcie ładowania typów ubezpieczenia';
        console.error('Błąd:', err);
      } finally {
        loading.value = false;
      }
    };

    const openCreateModal = () => {
      selectedType.value = null;
      formData.value = {
        name: '',
        description: '',
        isActive: true
      };
      showModal.value = true;
    };

    const editType = (type) => {
      selectedType.value = type;
      formData.value = {
        name: type.name,
        description: type.description || '',
        isActive: type.isActive
      };
      showModal.value = true;
    };

    const closeModal = () => {
      showModal.value = false;
      modalError.value = null;
      selectedType.value = null;
    };

    const handleSubmit = async () => {
      try {
        modalLoading.value = true;
        modalError.value = null;

        const payload = { ...formData.value };
        let response;

        if (selectedType.value) {
          response = await insuranceTypesService.updateType(selectedType.value.id, {
            ...payload,
            id: selectedType.value.id
          });
        } else {
          response = await insuranceTypesService.createType(payload);
        }

        if (response.success) {
          await loadTypes();
          closeModal();
        } else {
          modalError.value = response.message;
        }
      } catch (err) {
        modalError.value = 'Błąd podczas edycji typu';
        console.error('Błąd:', err);
      } finally {
        modalLoading.value = false;
      }
    };

    const confirmDelete = (type) => {
      selectedType.value = type;
      showDeleteModal.value = true;
    };

    const deleteType = async () => {
      if (!selectedType.value) return;

      try {
        const response = await insuranceTypesService.deleteType(selectedType.value.id);
        if (response.success) {
          await loadTypes();
          showDeleteModal.value = false;
        } else {
          error.value = response.message;
        }
      } catch (err) {
        error.value = 'Błąd podczas usuwania tego typu';
        console.error('Błąd:', err);
      }
    };

    const hasReports = (type) => {
      return type.insuranceReports && type.insuranceReports.length > 0;
    };

    onMounted(() => {
      loadTypes();
    });

    return {
      insuranceTypes,
      loading,
      error,
      showModal,
      showDeleteModal,
      selectedType,
      modalLoading,
      modalError,
      formData,
      openCreateModal,
      editType,
      closeModal,
      handleSubmit,
      confirmDelete,
      deleteType,
      hasReports
    };
  }
};
</script>
