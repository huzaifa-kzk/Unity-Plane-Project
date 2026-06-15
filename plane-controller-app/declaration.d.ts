declare module "*.ttf";
declare module "*.otf";

declare const process: {
  env: {
    EXPO_PUBLIC_WEBSOCKET_URL?: string;
  };
};
