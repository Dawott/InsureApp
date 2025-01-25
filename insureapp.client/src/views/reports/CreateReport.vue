<template>
  <div class="max-w-4xl mx-auto">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-semibold text-gray-900">Złóż nowy incydent</h1>
      <router-link to="/reports"
                   class="text-blue-600 hover:text-blue-800">
        Wróć do raportów
      </router-link>
    </div>

    <form @submit.prevent="handleSubmit" class="space-y-6">
      <!-- Report Details Section -->
      <div class="bg-white shadow rounded-lg p-6">
        <h2 class="text-lg font-medium text-gray-900 mb-4">Szczegóły incydentu</h2>

        <!-- Insurance Type -->
        <div class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700">Typ Ubezpieczenia</label>
            <select v-model="formData.insuranceTypeId"
                    class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                    required>
              <option value="">Wybierz typ ubezpieczenia</option>
              <option v-for="type in insuranceTypes"
                      :key="type.id"
                      :value="type.id">
                {{ type.name }}
              </option>
            </select>
          </div>

          <!-- Customer Selection -->
          <div>
            <label class="block text-sm font-medium text-gray-700">Klient</label>
            <select v-model="formData.endUserId"
                    class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                    required>
              <option value="">Dodaj klienta</option>
              <option v-for="user in customers"
                      :key="user.id"
                      :value="user.id">
                {{ user.firstName }} {{ user.lastName }} ({{ user.email }})
              </option>
            </select>
          </div>

          <!-- Description -->
          <div>
            <label class="block text-sm font-medium text-gray-700">Opis</label>
            <textarea v-model="formData.description"
                      rows="4"
                      class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                      required></textarea>
          </div>
        </div>
      </div>

      <!-- Documents Section -->
      <div class="bg-white shadow rounded-lg p-6">
        <h2 class="text-lg font-medium text-gray-900 mb-4">Dokumenty</h2>
        <h4 class="font-thin fill-neutral-300 mb-2">Akceptowane formaty: jpg, png, pdf. Rozmiar poniżej 10mb</h4>
        <!-- File Upload -->
        <div class="space-y-4">
          <div v-for="(file, index) in files"
               :key="index"
               class="flex items-start space-x-4">
            <div class="flex-grow">
              <input type="file"
                     @change="handleFileChange($event, index)"
                     class="block w-full text-sm text-gray-500
                  file:mr-4 file:py-2 file:px-4
                  file:rounded-md file:border-0
                  file:text-sm file:font-semibold
                  file:bg-blue-50 file:text-blue-700
                  hover:file:bg-blue-100"
                     accept=".pdf,.jpg,.jpeg,.png" />
              <input v-model="file.description"
                     type="text"
                     placeholder="Opis dokumentu"
                     class="mt-2 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500" />
            </div>
            <button type="button"
                    @click="removeFile(index)"
                    class="text-red-600 hover:text-red-800">
              Usuń
            </button>
          </div>

          <button type="button"
                  @click="addFile"
                  class="mt-2 text-blue-600 hover:text-blue-800">
            + Dodaj kolejny
          </button>
        </div>
      </div>

      <!-- Error Message -->
      <div v-if="error" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
        {{ error }}
      </div>

      <!-- Submit Button -->
      <div class="flex justify-end">
        <button type="submit"
                :disabled="loading"
                class="bg-blue-600 hover:bg-blue-700 text-white px-6 py-2 rounded-md disabled:opacity-50">
          <span v-if="loading">Tworzenie...</span>
          <span v-else>Stwórz raport</span>
        </button>
      </div>
    </form>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';
import { reportsService } from '@/services/reportsService';

export default {
  name: 'CreateReport',
  setup() {
    const router = useRouter();
    const loading = ref(false);
    const error = ref(null);
    const insuranceTypes = ref([]);
    const customers = ref([]);
    const files = ref([{ file: null, description: '' }]);

    const formData = ref({
      endUserId: '',
      insuranceTypeId: '',
      description: '',
      status: 'Nowy',
      additionalNotes: '',
      decisionReason: ''
    });

    const loadInitialData = async () => {
      try {
        const [typesResponse, customersResponse] = await Promise.all([
          axios.get('api/InsuranceTypes/ShowAllTypes'),
          axios.get('api/EndUsers/GetAllCustomers')
        ]);

        if (typesResponse.data.success) {
          insuranceTypes.value = typesResponse.data.data.filter(type => type.isActive);
        }

        if (customersResponse.data.success) {
          customers.value = customersResponse.data.data;
        }
      } catch (err) {
        console.error('Błąd ładowania początkowych danych:', err);
        error.value = 'Błąd ładowania formularza.';
      }
    };

    const addFile = () => {
      files.value.push({ file: null, description: '' });
    };

    const removeFile = (index) => {
      files.value.splice(index, 1);
      if (files.value.length === 0) {
        addFile();
      }
    };

    const handleFileChange = (event, index) => {
      const file = event.target.files[0];
      if (file) {
        files.value[index].file = file;
      }
    };
    //Dane do uploadu
    const uploadDocuments = async (reportId) => {
      const uploadPromises = files.value
        .filter(f => f.file)
        .map(async (fileData) => {
          const formData = new FormData();
          formData.append('file', fileData.file);
          formData.append('reportId', reportId);
          formData.append('description', fileData.description);

          try {
            await axios.post('api/InsuranceDocuments/UploadDocument', formData);
          } catch (err) {
            console.error('Błąd ładowania dokumentu:', err);
            throw new Error('Nie udało się załadować dokumentu');
          }
        });

      await Promise.all(uploadPromises);
    };

    const handleSubmit = async () => {
      try {
        loading.value = true;
        error.value = null;

        // Tutaj się tworzy
        const response = await axios.post('api/InsuranceReports/CreateReport', formData.value);

        if (response.data.success) {
          // I tutaj dokumenty dodaje, jak są
          const hasFiles = files.value.some(f => f.file);
          if (hasFiles) {
            await uploadDocuments(response.data.data.id);
          }

          // Redirect do szczegółów
          router.push(`/reports/${response.data.data.id}`);
        } else {
          error.value = response.data.message;
        }
      } catch (err) {
        console.error('Błąd tworzenia raportu:', err);
        error.value = 'Błąd tworzenia raportu';
      } finally {
        loading.value = false;
      }
    };

    onMounted(() => {
      loadInitialData();
    });

    return {
      formData,
      loading,
      error,
      insuranceTypes,
      customers,
      files,
      handleSubmit,
      handleFileChange,
      addFile,
      removeFile
    };
  }
};
</script>
