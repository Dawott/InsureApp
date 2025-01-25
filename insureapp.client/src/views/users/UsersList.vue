<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-semibold text-gray-900">Klienci</h1>
      <router-link to="/users/new"
                   class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md">
        Dodaj Manualnie
      </router-link>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="text-center py-4">
      <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
      {{ error }}
    </div>

    <!-- Users Table -->
    <div v-else class="bg-white rounded-lg shadow overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Imię i nazwisko</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Email</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Telefon</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Akcje</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="user in users" :key="user.id" class="hover:bg-gray-50">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm font-medium text-gray-900">
                {{ user.firstName }} {{ user.lastName }}
              </div>
              <div class="text-sm text-gray-500">{{ user.username }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ user.email }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ user.phoneNumber || 'Brak' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full"
                    :class="user.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'">
                {{ user.isActive ? 'Active' : 'Inactive' }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
              <div class="flex space-x-3">
                <router-link :to="'/users/' + user.id"
                             class="text-blue-600 hover:text-blue-900">
                  Pokaż
                </router-link>
                <button @click="confirmDelete(user)"
                        class="text-red-600 hover:text-red-900">
                  Usuń
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Delete Confirmation Modal -->
    <div v-if="showDeleteModal" class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center">
      <div class="bg-white p-6 rounded-lg shadow-xl max-w-md w-full">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Potwierdź usunięcie</h3>
        <p class="text-sm text-gray-500 mb-4">
          Czy na pewno chcesz usunąć użytkownika?
        </p>
        <div class="flex justify-end space-x-3">
          <button @click="showDeleteModal = false"
                  class="bg-gray-100 text-gray-700 px-4 py-2 rounded-md hover:bg-gray-200">
            Anuluj
          </button>
          <button @click="deleteUser"
                  class="bg-red-600 text-white px-4 py-2 rounded-md hover:bg-red-700">
            Skasuj
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import { usersService } from '@/services/usersService';

export default {
  name: 'UsersList',
  setup() {
    const users = ref([]);
    const loading = ref(true);
    const error = ref(null);
    const showDeleteModal = ref(false);
    const userToDelete = ref(null);

    const loadUsers = async () => {
      try {
        loading.value = true;
        error.value = null;
        const response = await usersService.getAllUsers();
        if (response.success) {
          users.value = response.data;
        } else {
          error.value = response.message;
        }
      } catch (err) {
        error.value = 'Błąd ładowania użytkownika';
        console.error('Błąd:', err);
      } finally {
        loading.value = false;
      }
    };

    const confirmDelete = (user) => {
      userToDelete.value = user;
      showDeleteModal.value = true;
    };

    const deleteUser = async () => {
      if (!userToDelete.value) return;

      try {
        const response = await usersService.deleteUser(userToDelete.value.id);
        if (response.success) {
          users.value = users.value.filter(u => u.id !== userToDelete.value.id);
          showDeleteModal.value = false;
        } else {
          error.value = response.message;
        }
      } catch (err) {
        error.value = 'Błąd przy kasowaniu użytkownika';
        console.error('Błąd:', err);
      }
    };

    onMounted(() => {
      loadUsers();
    });

    return {
      users,
      loading,
      error,
      showDeleteModal,
      confirmDelete,
      deleteUser
    };
  }
};
</script>
