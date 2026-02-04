import { Stack } from 'expo-router';

export default function RootLayout() {
  return (
    <Stack>
      {/* Home screen: no header */}
      <Stack.Screen
        name="index"
        options={{ headerShown: false }}
      />

      {/* Controller screen: back button with text "Back" */}
      <Stack.Screen
        name="controller"
        options={{
          headerShown: true,
          headerTitle: '',       // remove title in middle
          headerBackTitle: 'Back', // text next to back arrow
        }}
      />
    </Stack>
  );
}
