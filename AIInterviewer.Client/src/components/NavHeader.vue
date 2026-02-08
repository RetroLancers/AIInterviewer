<template>
  <header class="border-b border-gray-200 dark:border-gray-800 pr-3">
    <div class="flex flex-wrap items-center">
      <div class="absolute z-10 top-2 left-2 sm:static flex-shrink flex-grow-0">
        <div class="p-4 cursor-pointer">
          <RouterLink to="/" class="navbar-brand flex items-center">
            <Logo class="size-8" alt="AIInterviewer logo" />
          </RouterLink>
        </div>
      </div>
      <div class="flex flex-grow flex-shrink flex-nowrap justify-end items-center">
        <nav class="relative flex flex-grow leading-6 font-semibold text-slate-700 dark:text-slate-200">
          <ul class="flex flex-wrap items-center justify-end w-full m-0">
            <li class="relative flex flex-wrap just-fu-start m-0">
              <RouterLink to="/ai-configs" class="p-4 flex items-center justify-start mw-full hover:text-sky-500 dark:hover:text-sky-400" ActiveClass="text-blue-700 dark:text-blue-300">Ai Configs</RouterLink>
            </li>
            <li class="relative flex flex-wrap just-fu-start m-0">
              <RouterLink to="/SiteConfig" class="p-4 flex items-center justify-start mw-full hover:text-sky-500 dark:hover:text-sky-400" ActiveClass="text-blue-700 dark:text-blue-300">Site Config</RouterLink>
            </li>
          
            <li class="relative flex flex-wrap just-fu-start m-0">
               <RouterLink to="/interviews/new" class="p-4 flex items-center justify-start mw-full hover:text-sky-500 dark:hover:text-sky-400 text-green-600 dark:text-green-500" ActiveClass="text-blue-700 dark:text-blue-300">New Interview</RouterLink>
            </li>

            <li class="relative flex flex-wrap just-fu-start m-0">
              <RouterLink to="/interviews/history" class="p-4 flex items-center justify-start mw-full hover:text-sky-500 dark:hover:text-sky-400" ActiveClass="text-blue-700 dark:text-blue-300">Interview History</RouterLink>
            </li>
           
            

            <li class="relative flex flex-wrap just-fu-start m-0">
              <DarkModeToggle />
            </li>

          </ul>
        </nav>
      </div>
    </div>
  </header>
  
</template>

<script setup lang="ts">
import Logo from "@/assets/img/logo.svg"
import { useAuth } from "@servicestack/vue"
const { user, hasRole } = useAuth()

import { signout } from "@/lib/auth"
</script>
