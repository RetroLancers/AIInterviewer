<template>
  <footer class="bg-slate-50 dark:bg-slate-900 border-t border-slate-200 dark:border-slate-800">

    <nav class="pt-8 columns-2 sm:flex sm:justify-center sm:space-x-12" aria-label="Footer">
      <div class="pb-6">
        <RouterLink to="/about" class="text-sm leading-6 text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-50">About</RouterLink>
      </div>

      <div class="pb-6">
        <RouterLink to="/posts" class="text-sm leading-6 text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-50">Archive</RouterLink>
      </div>

      <div class="pb-6">
        <RouterLink to="/privacy" class="text-sm leading-6 text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-50">Privacy</RouterLink>
      </div>
    </nav>

    <div class="container mx-auto px-5">
      <div class="py-20 flex flex-col lg:flex-row items-center">
    
      </div>
    </div>
    
  </footer>
</template>
<script setup lang="ts">
</script>