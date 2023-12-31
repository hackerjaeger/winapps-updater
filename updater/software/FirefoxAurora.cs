﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "122.0b4";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/122.0b4/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "07eca01d727f8ea6ab32cf223f518a73c8dd062ab42e165490051d3cba95b8075cfeec9bc3db3154f46d107feea5c3a951cd6ffc6d7ee10f9d417dd8fa5e56f3" },
                { "af", "ee71726a07255f280979b021bf4d5fbce850fee06083db0fdead7983e3ab48e970408e5a255e91dbc8399a5ac77e89f7470b71898fb64f0853d3d807ac431133" },
                { "an", "f20701eff3961e9ab8156fa3c56d4fd73a580b1dc706324fd88ecd9fd04831eddb1945bf2af30c7e6e369984f6073382a7bde5aa50ccd6e3c616607d04aed663" },
                { "ar", "27f47bcd8258fbc55763a6ac3b92ba01d080da492c1560e5ab0f34cd74b9aa6a9e3b88b69153631487c44b2e13879a12763e508d2f1f41762a3ab7dc66e6a288" },
                { "ast", "fabde7a414187ff30c308136e94d6457fc8fc50ecaaf2d2d6e18f59562936958ec343e6e563fe008861f892a9d3786e9453b858c65e55294d6fba3de494c341f" },
                { "az", "6b3910f1ad4d39e3df109c91dc6f2633e98667f1843c698c1aa9c0da4bfa597a4ce14af868ea93e9eff49272d8e0fe04f2b9ca3ae79a3a04377635894cf02ccf" },
                { "be", "3898f61a9d5189334f8aec3cf6a502e739db32c587aa23b6f00e5b9e3f258dcf183f719c62fcfe6c9a0421c6651bb6675c96055221106863d42309649697ab2e" },
                { "bg", "be9ee9d84ad159f975f9917fe798f4bd47f81672e0db0a9b0dd5011880f96b5474a936cca7ff66be7316225163d7307f74b167858e2bc5072e2882a9aa86f38e" },
                { "bn", "b19918549df842a6ed0e74700383b84725c15238fd16590aeca2bface5787ec51141feb21f4e9d6396c4271f546887412d4a640cfffeb87858c5b4fdef4f0472" },
                { "br", "7ff40cc5e87afe752c0b0954a62314663b0e065443e06ea5657429dca977bc896df0824f28b33bbd0e910e28265246de01e2de9c1cec291e6852601a888bfbe2" },
                { "bs", "c9b45bd8234c70164691c79eea2d6e8faba4764b683d06c2a27dee5f1cebad5ef4e523742cb37d40d816fc5afd1fde81c2261cf6084a1f584a880f8fdb669215" },
                { "ca", "bc5d183180bcbfaa16fc70e8b1365ca386099ab8cfce6dedd553607fcd6e782dd951dae755c42efbc544d7f9a26331bd30840c4d3b5dcbda4bb3b0ddbcf9ece7" },
                { "cak", "a710e40511ce4df06cb75026f326f5a84bac31a13d4b3736f162f8804fd45c2ec6915812fbe63919ce53ec6dc3ac59bf0493ac2d66935d5635bcb55f4fbc9b47" },
                { "cs", "56e24392824295af56399863291f35b73f50e84807fd76f48fa813d0db6e8fed874a8a769b706aa503db34c320d93593109c4e5150de24cc8046bc9bed2c6980" },
                { "cy", "5aa4baf89504986384aed49e11b23e233dce17009bc7da62bf0a37ad620c04147ba14eebbbade78c603e93a263ab9e2f80441d1a674a5c01d178da6a5dae6a4b" },
                { "da", "7138dfebcdf0ed74e491663afb36b94ac239664b6e02225a8ccfbe2512ebaeeb289fce5a701095c28477614a115ff4e7f1157f707d94145235f851aeb1f2c385" },
                { "de", "51d5634e6c7812cef99933b826de45f0996f64083bd662e02269bbda4dc067f42645b089c57bd7a9e0c5a22c53c8f7be9fefbcedeee609461d35d2aef3c65189" },
                { "dsb", "e835ecb140a7b2b87c91c2ec23fa0726b1b3f927f7f094ad416c2fc78d0e7e364d03f6babb18f63f2c022c15889749b0c02941fe9f68c3f03c7ba7e78dec7ca2" },
                { "el", "ca0c11db76755686040f830262de63bd380a75339389bdb8e876f9b2d1cefb31322406aaec4a8a30355c4ecb6240f8a21ae95839c393f418af90768feca3f0a2" },
                { "en-CA", "61d9d4dc70ddd6051a16833b00dda9abe9c8e3633d48d3ffd358d3133bac38510bfe8ee0b56fb6651887eccfc710f864e8c3a25beff043d2d93e8f265ada589f" },
                { "en-GB", "907cf8648ac8bbdfdf27f84e64b6b8200b5ec97ea8268a9216adc33902bdacb551af4a33d01316911c1a0c5eb7ce5c53e5723f0f8da8867d060dd6fdf8b78253" },
                { "en-US", "7c8060e915624901cf357e46dd1f54e28295bcabff141ca2f360ee30e19005c23c7099e129dde98589e8871e304b881d531c1ab694ffa80d62b049f3e4d636c6" },
                { "eo", "db1b653250cfa256eba37f0caca1f5f8c9638f7caaa4f747d93e7a59097f6415408ecd15c020c596dd6c974fa9ac24595e18bc86265ad187d803a8a1172204eb" },
                { "es-AR", "a0cb164f9897e72cc8ed351f00abbbb1cb8ecc6ba37717f68ffe8c4185dec58871d8460980223309b88519e4dd1cf129d7b1181f970f3477d0cb9f46731e53f7" },
                { "es-CL", "92425d648cfde176ecc1302cc5a1946d40637660774ff5b15b981cd47c0cac579a0c54bc1402b00a293776950becf8c392a874612c9877067feb681e8292c34a" },
                { "es-ES", "c39e43f25bd453c9138653ab4fc6590f02ae4719c8a371025fad7d22e4191fd01d08ed6124add029df352999ab0b67b96ddc1b792f902decd0ec1137eebafd69" },
                { "es-MX", "26294cdeeadb36c54c2297a45a0633da892407ff22cd4552e9746dcef2ca676bc3b985cc9e257bb5e0b339d90be16be7484d6196652fc49feaac8ac73ff106de" },
                { "et", "f4f5c9a68a22efdf1d0f0fb74061a2b4a45e610239e339fb7efe30e6b547d65bbbf6faf7713040ae619cdfa3e3ce03b5bdcb34d0374e3d752b90a427b69acbf9" },
                { "eu", "1ffe282bcb8f8070f6794d5a5d7f5fd9a063a476a47f13652cc7c776b0c6beeeb7f41e186288ab7271ada638913cfc675192e8f984f6e56a6fabf3262cbe31f9" },
                { "fa", "5b6e9b45a674ecbb49121f6cab1ddc49f4ba30fcb10a6d77054bb6f1ec5b55d83f4935ca762ecbf100c535d89c49e6f2f5f7335f75a071fcaf84329b7f073386" },
                { "ff", "0b606255559da5582c9d9202c6085223d279820941839bbd9784ea7431e55a27e0fa4c94636ce482e629c1fe4ac6c99f9a1b0480a89619acbf261f147681c065" },
                { "fi", "c62710bb0f5d57790f072d0978f5835ded42a4cbc933b3017aedbe28c5680f3c3cefbdd6ace6fed25a3feb34c0dcd551854b9008406851715be14a453af1723b" },
                { "fr", "3a57c0829e109346ea69eebb675ebbcebe0472a9318b8199383be51e854d8ac1406cbb1eb885593cd511e18fd33132fc29a5a9a8c9736c920da596cc93fbef70" },
                { "fur", "f9b3799ce14afe51ed7a4c851b52b77acf254a7b79975c4e020ace4abdd3b6597c40b78e18c8666a0389d1c1bbee91ee5a719817250d1881634b3fa446e630c0" },
                { "fy-NL", "7a2908920302ee78a6650bb78fc5ed9c57a595df82c4e30ab9df532ab8f95987a15126f2f1ec696dbfc6f8b8c301e5fbc8cc7d6f59bc20cf3a16691308032f31" },
                { "ga-IE", "676ae5edf7731f614bb89d8c583cb28b40cac58f84cc3dbcea17ef7325305e39681f1100b256fd719009773333008c0b4265f18e1b400ce3e8bb452b2bcf790d" },
                { "gd", "f238656331d2c02336f4d45da121ada46d14670ad38bedd571c34922e77897e3e52e9a155da0209168c82669ee8ccd1b8dec5bd626eaea73b65ff23e7c2f29e4" },
                { "gl", "3ab0b21947f91b2351dddf349c06484a226004a62db0e2c1dff5f61bc5749d1778ad850d4c5bc189ab0739ed9c0be4f923e9f9a999549ad000c6f318808b872e" },
                { "gn", "4779cc5761b07d31eae8b181dc41408728545977ad99d137c5cb1c1a952234f8452c9a6cc4aa80d4abc579c539e28f8de8a5fad8653d5bfe914041af3b2d3fff" },
                { "gu-IN", "8d915387d627520d6650d38e5576244e81312a374cb89e9b2725abcea78758ea585caf79df78d2a256503fccc75f2ee6112fa266965ea996ced527bdc1fbe615" },
                { "he", "bcc3d6365f3475e84dbf385585560310f8726ca77079e904b6dbf81893f7377abb383cb918327940e855c39ea6da7acdee8ad3da9da5234000747768d4f214ad" },
                { "hi-IN", "b3e0e540148cbe55f1bf5a486f5f094d6afdccf4f4899dd475d35d914b99c040fe560e64f96f91f745d9f9ca6ed900c7868144c5707e92738b0f72e27168425d" },
                { "hr", "969e22ce870b26dbe86a284760e8487f0494ddcee7b689be3c062ee1ed04cf906c26f226632f849e759ee007f7292d01374433cfc2565014fa427e30cf17be0b" },
                { "hsb", "7e64c35f3b1f0039e47e592842de580a5f06e0c3f9737d668e255e89d0d68944ae7a483d80698d0f2f468d9cc4bcf5662d11c06ee48c86a18c18aa4f9fbb848e" },
                { "hu", "7590be300593131fd3122fe7a910630a117dab5b96557aec6c780e6e54956ff6ab7112f963bce1da370476c9e6bc922c0db0aeb1f6c181db90a59f2de7de7514" },
                { "hy-AM", "acf8cf69de7ea4f534412954e49de421528d11b77b097d4eba36a02f8a97b86ff00098c6d08584b0cb2f449f6440278093413d949438c388c847bef974d04bba" },
                { "ia", "9cccfab60d415e305b96a66ab93e3dc309ea3379153d2c55b477c5852a6bb8706d33b44176b96dc6af5aea2cd565e1d9e859c188838c553b46da394163ee98af" },
                { "id", "c47ee44e5f7c2a1c56e80b18230741ffeed5ec6bab5d415d4f94fe0b4019d7d4bbcb85cf76dedbf526812638e75540dad5e94ce001248c23f83b2da1c3a9cf1e" },
                { "is", "fef59d8ade35c27c630cbe787ff33360b20c9dc4b76baa1f14c07368b525d2bd309e19feb7ae10859ff46d0d0371c2efb421e5412ef769773c891f1b333a810b" },
                { "it", "ed8f885301c3ccd16afebec4ab7fe40632a85b1fe1c4ca09bb37c7ef13139de60a243966ae7aaae22aa4a073c45c499710c43abf0274a46dfd8625b79a0c6639" },
                { "ja", "cf93b919678acc6def74f2612449463457ce77e99e759f4ab49afbf8d378086d8995f57bf5f5fd5f4befcaec26b230a7f8b66be3ca644857d770a71ffad4ed8f" },
                { "ka", "f47968ad6a40d26604fd4a8116ef6d205c48f798009337ebc1983df123aea67a7863752fc0a9973fb3baffa7e8bdfe2205e0f48025822272a340d7e8f6a44f33" },
                { "kab", "d11954dc826f9f5f1c53ac88a15f97f6a625b5e1e5b2a767ad5e30082829222292c140f475bd2d521b469660a172e93f5e9867a7c9fe1d3eeb5506e8aaf5cd58" },
                { "kk", "13f77752a66b137025eff95fb98d06055680c91cec147c0f787d1309852755956a1d1b08e26917c332d89b363abccccaa83b05ef5763091718c48bc40a627d34" },
                { "km", "abcd60c5d77e58ba970e4269267c72e649c17fbf28d7a162b0f9c45e8ed1edc13cf2a89e5185c531d5f4f79078ed5496f056b69db84e09a254d01abc7ea70381" },
                { "kn", "ef1a06ae2e9a5250698f383d402d755fdd7dc3889dcd49f0d015ed6ac21f36c8a3a7eda2f361caa8814bcf3281c0b84eacb4a445cee304c2c41012f7faec0456" },
                { "ko", "735b658e7b39611323c27d80da247a3bea94be7d5b2614ca4f7366ec17db21c55f0daa5115aa06200acc75c772a55e3294ab3fcbeb0d9cbdd3efe3a11c0b02cb" },
                { "lij", "213630e9f588815d56983c243e20e6ad5579701925f83417173e61ffb85dcb52876f1ed587ed8314157d67cd74bfea659201abb95d48e3978955dee25e243202" },
                { "lt", "487bc9e88c9d60bc03d6d820d3010c750812d93712a8dace1ff3f54dfd35adb51b905b322c7e343a7172f7778b5ec1a90b89f5b0301f65b5e2e56e0a08cfeb75" },
                { "lv", "85b154511bff08c466375618a8843498794a452e39607519efff11d863f0cb4c174a2c7ac7e11c7b16b8ee9c080ec04f6dd3a8bc372c1951bddcd4fc0f2eea43" },
                { "mk", "a9109037dd287438f284fd00a613ec293bb62ab6c74a17f68860fffb2f8b7ed829cc4991c6de05495da21e76a1f59f1c4c2bd0b11dc6a116f868259b9a5e3de0" },
                { "mr", "a9df11a958b7579c883a5b7876a86265ad27960b1dae000aff48b3cfb5bc54a90afa1379d04f96cbe591d3e4b3444bc4940173476821f5b03547c0fcf956a710" },
                { "ms", "974233b487926b96d5ff0c35e03838e377760be71e624ad0f91638b72d151fbc146703b5e8d4603740a75e57e9d1223419c36ed2bd96ec2e145d70955c3af0a7" },
                { "my", "b99e8f44afbb8e5a8d61dfb6dd3b7f5225417afc6be9f6f5281ab154a9f49522ee33a08519ba1e310824757bd75922145576f2eadd4ea52a2a38d5eece991e67" },
                { "nb-NO", "fdb75015e0293260bccb7dfc3888bec4408c3632a11bbab3efad5da255450f40819368182cbb4631baa3e4a059f342185e68842095c83beb9b89cf90b2518720" },
                { "ne-NP", "aba878ccbf03568dd98911863c6908978e2d5025c6377036f7c388bd60503e7e28c3cc04f70b1940b04ae4c8655806fd42d29a4eea427cb14ac914f15259c7f5" },
                { "nl", "3436e5b4e18b3168e88d4c768c4a88999ebb8b0e0657036ec1ac894ddf01309360712bfd622dbc4c917987c77cb6754d796fec10337591dea2a1c84398120a0e" },
                { "nn-NO", "3b0575ea8b554c527f642f5dab976df37a855f3083f4b6b6df1b82e5b14360275b9675eff3c1507c9295d16c8d7545299c9fa9cb265a967bfd816ae410594392" },
                { "oc", "9f8e868ea66f7af7aca25f9a22db7675c476252c5562396476b2f41d16c2ec679497a4102281b8e4c9f16511c96210733549b8cb6edb075666f9e0d1dd4c718b" },
                { "pa-IN", "9b9f3f43b22c3de8cf6dd9c8aab59b1a28e27816a9e8b8aa1b47ef061b48e25ca0f8e16c825492a9f40eb1a776ebbe41bd134f0514797344847e1d4ad66a93f9" },
                { "pl", "5e7211531dc7d002ba98a1d0aa3537f58ec34c11374435d720cf21b114ad7ed885ca9ab8efc255dd79eff5780128ee9435edb1c900cbbed92f5cdb8731a7b91d" },
                { "pt-BR", "5fac1c62779641a2c62281a2e11823fac41a4d53719744cf3c02b8f03158a4a442024f4eb4188da92801389be406af8b1dac72367fc7ba7a0857e55779eeabce" },
                { "pt-PT", "130910ca2834494be89a956d03a3ecb8bc3fe56e84711dbb5afcdd86194729b87c2ebd9bc411963b7ea11ef9d0d088199d83617c0ec7f485a97e42cef2d31041" },
                { "rm", "400852e2388510621148e00034eb2ee1858e0a1bddbaabf542ab94954d1f608a2b57439b02f9533cda9edd1f37a8abbac996d3251853825b9921e7eedf639ea0" },
                { "ro", "bbcb420af6f289e720f58d2bb6ee76577e763f44817740e5b3d6d1566e8a4cdc8d383a1b8756ccdc9c032d0f91be2ed382d21e3269a34a528dd3bed112541902" },
                { "ru", "8be8059a9e2bdfdb937374156f4671f09ef4110c7fbb5c556e95af5a3d76aa4eb8fd58c1825cbe0b2b92fcd96a773f4434df62c63f6237aea866d2a150f8c40e" },
                { "sat", "24cf6a203e1b5d9fc76de64126dd356b975c4d81464ca4635650efe766dbde9d34d73dd79f2d15876c4a169aa4445934c0b3954da7a6e555aa19146f40725ac5" },
                { "sc", "6e027b37d694ba2dd689a8154a8c4e0fe9e7a0934b8b5f265836be1d36a6a47d28d5df6497b6379cd4d2bb365d4d95b14e726a0b2528f10f15d5452d068cd74f" },
                { "sco", "414983bbf59dee42c06407151d6c8001f39b5d2859ba496d59cef873768aae8bf4d15e959304615f57d1b60d663fdae28ffc932daf0ba510941c2a0e44fc1a57" },
                { "si", "1ae8fa23404d2962cedb3a2980b951ee5a159ffedde39f52aa67ae915002d18ad42520d485ab5ee26083cc620e5db160cfea16e9f9c2115b5cc0b0d841e87859" },
                { "sk", "91775b190c6d375d9347d40b41a01f6a681a118e27aef4778d2e83a6f9099fb8c1f502657405c079f92a07d7a5e105edd4487e3baf53200eec7a88b9e6382531" },
                { "sl", "6c43d3102e86c92ef7d1b9407a46eb77bc5d57761367efc954061e74176296af6161c11ab81f18e58446758a8cee8d23c8618a149b283fd2e351a3a83bc92556" },
                { "son", "178dd1a8d955662cbb29e7b22fa633bd1c742fe7aeab59ea5ec69ecbea91d5ffa1d839ec5c4c04f576a37b7ea5148df09f075e654f82c353bb60f8b5e549ae68" },
                { "sq", "a69f81b40f8dd60b221724f072a9932105585aa3de6e58bce291de9d5a353e9d0c6b5ffa0c152e83ec977c45ccf0d4ce9c53602b44eda023d34e09377bd3475f" },
                { "sr", "af868a52c1a0b6f9588c95f144e26e556a7f9fba52ae4312e5191ae06ed386f63e0fec05f54389a7643a6c040a2eeabda7db81ec5bb3cfa17989a1c81ca250df" },
                { "sv-SE", "4d161aed323df8673b6d5aec30cadc87e82f2eb2b88c280040f41fd219287b8aececf058394e371a6b84f5ea7c9a8fb609a654e621c49ae0365511fdad2d8c07" },
                { "szl", "16bde8198c2dd677c08865156a915f5f8c220026e4f57007b1f99712bc65ffa5db587c7d3a2a13e8c517707f10ce642610b15c69984b73287b6bd3a797c5b7f5" },
                { "ta", "ab2ae8d2bf75aa69d024dbcdaa9e4c38307ba68a9a2fe7bf6654029a9c58a93fbe6d7d509cd0486bbe60e1d7ab425ae8b4070bc894388ab4934849996d4d0cb4" },
                { "te", "b903c0958a67ba7f05569702b4d8fd7ed5ba35f80213443c7119ef6d2778cacbdf3f6266f5f7a60d3ad9bb60eb3f2b055036097cfd211f4838d2fede7df1fb58" },
                { "tg", "ff8e986f50b78e57ecf52ea3fc8731fe6f3b0342da0723541accbeaa04df0dada5e9279184c67443fa351e757a52cb506def1b7cbf44a9043799f46b4a48232f" },
                { "th", "00dde184545e78a05e51c6f33bc3ce55a7c2cd5186c84462afa1c199b71e807864fc271c1aa1dd034af2dd48fb4df6d3d16ea389a6a26c379c442fa6c8b505ce" },
                { "tl", "feb249199e05a0e2f1a49eaf15fff761a3078cb4177519dde868f0056de543d4564dca983d95ecd47cc9a7736de7c2602b7154588b8fa4face73f0846e3af3bd" },
                { "tr", "806eb5b28e9f69a901252498ac689beff6e914ecac86587a989501605dc09633c4e02e6f5f5e74eada0af8e2b81d9a63a30c5992ba8e0ca0dc27589464f9a454" },
                { "trs", "d0d8af5446dc448971252e70431a3fa16e23b34fb86b25e21ccd5468ace3c3bb9789438331cf18779f597cfd716380a06365b4f656d6a42115f244f2134a145e" },
                { "uk", "74c1cd50feb014fed45f00e1944c2a35e882891176a4b09596d7833e3152c1d9f3a30ff62817ba93a743ba4b682aa83efc4c479256325e48baa8b5fb2d2eb9d4" },
                { "ur", "70e7480063de0e061212fecc62a344fbdb03eec4a27c495cdcf178de5d995b71a80d714d8f0d8ca26a01d096e097fe99f3f9db5a0480442329c92d4acba71202" },
                { "uz", "4433185f26630119d622c81e5ee2f2e6d77c0f8c7c9140be50dfbe59934d5fb8991d93d6b6a30beac0dd61c3d5e098a70276b0c9764bde5fb34b3da3ab4b3377" },
                { "vi", "642329880ab3b3846847fc495509c549e98f267f60b45d28b5e580c2f5e21f76d188b36b5040f89797c5d72fa1431132f7f099286510b9337bfd430eb1ee9591" },
                { "xh", "b1f7696b79b96fcec372bb7e132c9e6a819685926d25d1986d4ddf43f2790932567b272ba6ab21cf5a55014deeb2e385a1a5d7879f2a5e47bf93003edbd9b366" },
                { "zh-CN", "2295de31d1f89326bd9a88f69b8097e759f5bc8ea7788e8884d64f00dc05bb7e815ec135c0916f815e4a50d03395696ff7667733b8301974e2b0e5e9d2bbd0ba" },
                { "zh-TW", "fea25c1bb1b36ce1311a19f5d10ae72b594c2dc071a1d0226104e732ba3837729c62ca4703d7672200a83a509b0a77eed364fc03b50a320a0dd2593feb84f488" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/122.0b4/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "b33de6cd9675a235f3404b758316bfb7b1b004944d0c9b6fd38033024122abe86136450ac38eeec5d010c10c2b7e127ef429b4e094cbf47eda4d8dd320aa80aa" },
                { "af", "60862297c93fcfa13c86371ad3084ef318f3c0e6fe0c3da092750d90b2b9ecf8d26863cefbb2b2cd5633c789d129f9b7ec371f8957d4f2e97ed76fdfcad50fc9" },
                { "an", "7ac5fd949cd04728e7249593ac0773919d138c4716cb6268f159df7fa9c6bb89ba37b8ffe3813daf1fc865f0dc1099e6ea9fc42cd93828b28d5562c4233fbe14" },
                { "ar", "1fde575e47f8e5f045aaadb7cea4cde2f9f98aa2e6164ee935a80bcd9648b557e37584e654ae16e59d28753d8f53a582a803254696081364845f61933cd5b79b" },
                { "ast", "5a223db6825c38de380e8530d08683f09569354a28a1c946f5668eb2bdfa01433a1126517f2eed7d345df595eb2249518557ffee0edc48d26db15451de674362" },
                { "az", "84ae5c4efe2ac7f6fe5d26922a0cb8009f4a852bef58574778b82e35afd545b3e159a3cd88463a1e3456d807dbe9773c356300afdd3b12efb4245ea7116a08a4" },
                { "be", "7e4a19e3911e27b406e6d93c0a9bbd20b42c2dab4819e5556a1bcee365a6f1453ec1b3e876262c821c91f167a8331be1e6056eb656eeba0aa527820db8a209ef" },
                { "bg", "427c79291ea5127446aea6826dd5e4a7d7aac8323b9112cfda2cdd1c83bade651ebc39306c96571ee4e8cfd34f2764d44013995995147a793a88c242145abfe2" },
                { "bn", "5733b1bcf1422ff8033a965c39626f84b435d244caab1e25e1eee571969efc4abbc0585b78ee3b95f8f66d6f4d65aa3d627b81f9ef80b77b9b1e6c1a1c3f7a9d" },
                { "br", "9273a6f2ad49c73568a805815defe64313311f142e849d8587f3973b4801ef30d2c25679c019397e4ddc56cdcb0edca6c477efca215201fa57ac7e6583aae612" },
                { "bs", "bfb30dd8032964226bb99d69f14ace9d9f69dff73088b254b754302fefbc2ea57b39aed7707b39bd31a49d273bd572fc2694766039dcb2899263dbb1e022e717" },
                { "ca", "2280f7a92431e47f109b764929c42b374f942220fad14d57e45d705b39d05585b1a9ba52f152cda7ce2efab2b4b1fa8efe361036eea1915c7be674efa58b077a" },
                { "cak", "77504c0babaa68fba9259c603bd68c44b5272f65aaf1e7726c8aa4e2eb8198d4a148cf248aad4d3d5d27508920e8195635213c12b6b821d12ff362ea8ba256c6" },
                { "cs", "e9dfa8667c5e73e5583e3450d8243fe1ddc2e23a7ae45be83c34dbe52b5dda8a5984aae1679ba0542bc4254748b33e398415fda587452060f7d9d9b9d3e3aaec" },
                { "cy", "ebc3669c2003fc189e4d7fc89030ee31b80339360afddf81456f051eaa3d7fc5ccdc7ef9a1cff2db27a8cd76d18601d196b98609ae4449806e131fcd641b2f08" },
                { "da", "70e04f2d71f9ae1ca9f0143f1e37dbf4c988193bfbf37a011208891b3a843090beb4a8626828c33c75d58f6a1851ce56b5231507600be24b18a3e5003580dc7f" },
                { "de", "7e2418b550c9f621046a3398f84e442c9ec77ff37ff49bd3d0785b5f70cda512ad2df21c5922c3b5ba6639afd511897757ba297d47da04b0ef6e013d7044f010" },
                { "dsb", "aa94fdffc85a2a91e705eef34d3c6563578cbbaa1b449f21757119845450b869c9372f4249ec3e6a9f1fe0746ed39c311352809927b2c535420e2420db279b7d" },
                { "el", "560a96f3d6b71846a3923b455715ef48c7c996aeaa14f9edcd0dc0d4c759e08ea367b5dbb4ba5b11b7700f8307c345e146c3670847d8d7dddda87d29b82e996a" },
                { "en-CA", "51ddbe2b8f4a81a06f797afc4c0828efae516a5467d3ffa27e8c01ead109c941006e120da93272b5af34f8369cc999d4f47dc07e88720725b90fb8fad26f74ad" },
                { "en-GB", "b475445849f5830d3f5f97ea050fa5ee86842a8c63d5849cbc5a649c6e6c23bfbae4169873e1576187e4569ddb742738131b0ea22d28400a7188701fe864b724" },
                { "en-US", "bb793c2bcd83292ec2b226bd1d4f3eeb44789ed52fef528d25170e39908b258c2bb8194173c37f8e303dae780d25c282ae28c1239944c2c6607390275ab1ebae" },
                { "eo", "31577ce992a4f8dcad4f609e1351b2e891ef1076d63985866acc2afac01a8118d6209c1d8b95839d813f29462e8fd1a9edf865db77ac4a7e84112a4b54614b75" },
                { "es-AR", "8a5707d5d579a674d298d5ca410d9c331683b8fd28120234ba3fa5b4ffdda5919ad8528b09491f18a5191c12b06f1327474ee491647746ac379a425b7e5a4bdb" },
                { "es-CL", "1a58a4c3ed4c9a9c4aa7fe82b6c78cadc284646cd298750905e2ea06961a42b54dc4a83a4ece2d00148d80261be69c39eeb945ccd8a3f02568538aa10b437704" },
                { "es-ES", "8e498f7855b5f460f7d7a09d2b775a24f14a0b93df2bd76fea967743d3d157f8bdfbff231248299c2184a3ee3fdbb0cbe8ff124b8ec102021fa9239e4de7480e" },
                { "es-MX", "30f61cd806315c2d86830b02794a784db5e214723cc98b28823f691ca638f53c3cc9e0bf8d03b958e9b05cdbd62f755433eac4e0afa29cc45c1f314937d8aaed" },
                { "et", "e19ab7e80c27a5109dd1670ade8c7d6833b19080100f69739748e25e70bdf54999c9d3b7affed52ce30287e241f2937d69e3eb90bc097f5953a06d87e785941a" },
                { "eu", "07dbe24fb3da55a73812697355ad6d767e60aa6c12af421f422214e20d18ebaf172289311f0cee8352134175c3db5332e0801295ecfb44f7ebc2b71b0634f087" },
                { "fa", "67038beb62023941cd581fd288ff8d95acee5d4c879b9be67e181956e2fd71065db4d66dadfda65e70ac0c81934d9540e0b81953f48915aa0720c64fb089f3fd" },
                { "ff", "9665e4e0924c19a585afa28541a289e5bd4b5d9003cc879cdc813cec83b57bd056400644d98200c586b65fd6cef47dd07ba7d4156ed419ce7762b1c385250ba6" },
                { "fi", "d212337eb0e10ca93916d897fce8ebcbac60a1203912b00d2dfa685bdc450d28c50f0a6896738801f8d2322f8a83a65fdd1d21899e110ebc73e47080db46d31e" },
                { "fr", "e786f12e3068feaff15e21ea3cf67fa9ad2ea41ed026fa1ec35744d6adb9ab7698bff4e1eaefc8b08e44d6261d09b26a52f4a077d28b0a23056267032597f9e6" },
                { "fur", "13b48d6bbce8cb5cf43f578c726fc26e1ac1089cfbefa33e546a8d1080737c10bae585a9c88e3d32f19d09f0b91e31616c55f9a71bf5219109f784dc43305a3d" },
                { "fy-NL", "6fa7e08f4e6cbd45f73e7d56ebd5d48dcb67c66256d5fe970535b139e098226bc3729c089c2f0c15bd4ca32876a85bcb1c34a165eb1e43ac30471051c520983e" },
                { "ga-IE", "e2e8514a4878b65d088ccfde2865dbeb3a53e979777813ec86498d44a8eb339b7f1d23a9e34b391d8d6df7c795a854e2a832c31fd9e40c0f8202865a1dd58ef2" },
                { "gd", "c381f3e98e801390d522eb02b773375a08f7f6a9b1fa23eaab874e63cfd871f5313cf04b4e2e5bd588495dc8ca4f352002faae250f990e8c34d3764373e432be" },
                { "gl", "275daac6232d3c45225ab134484ac4e0c9ae9726422c200e9b49f61f8354dfacd725b360bc794523d4902f0e5991794c85bf72c03edeb952a4ed1b1a96bebc92" },
                { "gn", "462aa8d015956e91e9c0c96cd7abe44cbe22a92b3698d2fad49dd6a2f41a595fe3c4f02880cc876b64e61df7116dedbf33b2bfa0808b82ded0ebcaacb905dd51" },
                { "gu-IN", "bcd2c681f7482b75ca0383665042873f9f1f2673f7337c59013d6d0448b7ed4af880b8854a84077ac37a9e82bd0c89c32cd9f9f7b8f44000e09f1e5a04b12f18" },
                { "he", "ac5a2de8cbd4a3166b356b9a95f5a95ed9428f65205ba7d5d1e6aa9074f4d77c566c1dae9e34b9115a6b846106163d456fb42f9648bf04f33777902d5e4ec1bd" },
                { "hi-IN", "29ee433da1771c1cb91a7dc40bd0bdfbb6d993f2fa76799b7f87caed4592b6a8642aef4a91616e5b92c2eed8d7956891cdf2ef935d6590e3a4e8d659e9e86195" },
                { "hr", "da8229cb06d3a02b8d3dea41ab8057929f1ef3b86a6182aa3991bf608afd4c3dc8356df2707192a6d307dcadf6b4b91f57f889268eb41882bbc78947efb3e635" },
                { "hsb", "357e2947a49236f6a5c5f0112ce03c790b580697920eae47b2b5c32586c8f5f316c3c169b82dbcd2fe7dfd6402d90f041af76d2344c011e4df68e2d9a1ef7b84" },
                { "hu", "64fb13004cf8df8b591744540ecd1c9a61240ce0d6a0853c12eb1a4188d5335ccbed0e2329c86e6019eb3854bf985f1f1c62f69aef928f9ee7b8e031842b4738" },
                { "hy-AM", "13061110a660d018b126f1ad52f1b7f679a8065714063deb6e2adb24dad8d468815a05dbd2d450cb49ba77b1ce98a8e4599bd335bfd352cc8567c5ac41476652" },
                { "ia", "388e5071478bdf727d221e0143458d7a77a47e367c7526e7129c5cc4ff74b125891977255e7c77cc3899ef77edd83532b243e7fb79ebea1f86bcce0b42507dd6" },
                { "id", "fd09c36e355808b7cc22baacbb071a686cc5781077d8b35c43a3691e59273cce66ff69fd9bf46f6d603a51ca3cc053cf3cd0411904c9b421d07f5f2af6e23be5" },
                { "is", "a815d4f5ac6effcdb11634cc1738f926f0f8dec5922d760f26a1329759b927da4e876989b84e157a6b5f85de9f6a453e0baac74aa0609e06a91670a8b2cce08a" },
                { "it", "a1a4451bacb4e3fd7bc163695afd02bce8b67bd754905e0bfdf7a7719996d017be18d2ac6773477e7a7eec0f7e777ac8f8576614f8847b723e9a5d3174d72a6f" },
                { "ja", "42c76ad65edfc1c36969aac889d9ef53eede8e6241dbf87cf4b387b4fe8774a4246e5f069466813054a2f383ff6cc1efe23c14d0ede04ab8482c330c9003c29b" },
                { "ka", "08e796261c866ec2f1633979f5a11be9f35832efae7c654c8373abbf190637b8c6612daf39f495f97f0a0e62a571f99cd770a0833a3f62d8c6865a2a18b76592" },
                { "kab", "8498c173813f685f58c76c45ea3d7d3dd1811682319508f9bda0e1b79dc3fb1ddf731ed3983520e282422e637e56a6a81202f8329c1397371df35111671ba6ed" },
                { "kk", "2b1854d4b7fdfc6e85bfd8f8f3bb54b66c09231e7cc43a9937b1ed212ac334a848bee89e47a54b95951a65cae3d4b18abfe3281f11d4ba06896c73ff19612304" },
                { "km", "634ece1dcfb7ac8f23e781506f74f11face4dc2224f635ff49d6a86347b067350308e072129ae3182324c29cdcf509b925390050d3e8459bb3b4bc8852e88d3e" },
                { "kn", "eba3467ba9c45511ffa97b64bdf2f30b5d72dfe1cf26215ef2de76020d7cb56c0bfc08724180540c259937d8c486866022dfa5c69b42a1bdaf84e6dfd17a455d" },
                { "ko", "5dc6002584b7ba70dca8493253c33fd34054a531c2a304f01b651fa78fe59c292fced7049e44667c8d19e0984149e9a7f862a1c0052138277e2f15bf7f2e3db0" },
                { "lij", "d14032c157d59104b6ada5e266cf19ffc1f27657c77cef8ba51dbc6be0b8198dcd82d406c94242bcac9df3214ee993e41a5b017a62dad331f7f1fb245e89a254" },
                { "lt", "e72a253b60895a7c92209429ccc71a9f75ffb7dcfe885b39ba05fe462b56738aa03db5900ca9820987663c82a418af89ceb861413faa41c7615b0ce74218c351" },
                { "lv", "3a0b1be8488ca463e9957d64e562100e7213e4f010c53ae0ccada0bc199d73299873c59ab1c2896ebbd547cc25997f0802a8342f60d0fd65c171f2f4120fbef5" },
                { "mk", "88c30ff08602a97a89244283da00f4768e0b7a775a7567c0c0953dd30afb2f50495287dde521adaddc43317dc29d03832eb63bb777939184768f01d9911a00d3" },
                { "mr", "325da48ada5218c26038227bae8af35e05134a4b5174991025cbd5b700433a48d7641df3fef33e4bdfb0a6186aa0c88e6ffa8a1d14fc1530d75f8e76628d14df" },
                { "ms", "e8cd61c17cb487f14e11e91c72a58c55d531afcf51a0e7fc102a4f6e30a33bfc18116518afbade840c130e9f5f595f3ae85e9e35e159756f9888883bb94946a4" },
                { "my", "6b189c340a1e67b5c00d53f98476fe189a9b8f38bb07b81ae75b66f5db6bd8194bc41c95afb973450b9b897a1dac266bd1b677569d24ea875baf89344d718bf7" },
                { "nb-NO", "d7f677aebb871e5396dd232ece0697ee8f27442adc0ef6e3d91f61e67fd10e6563dce360770e28d75b68ed5d8194356ca324b0e09bfcb6503ae5a834be0b9687" },
                { "ne-NP", "d8220da1bbf0d2598a3b5405074f15add1aa65d1d8c2ff7970883ed699c8d5216546164028f2fa881476b29769cda9a93fac521d8fba12b7670612769f3cf17f" },
                { "nl", "dcbb5b11c523b9a8260f585454d7458104a21cb78b747988d0d4e343050c3aaac0b2d4af3a4c8503f3aedce86c3a0af99932a3aff311ebd9acae8d64d8b7344d" },
                { "nn-NO", "bea179797e2dff16ad2c0eb7299189f4d58f96916ba0b1058320cc3c356eab19a09cfe68cdd9f566456c8fbe75d2b747b31310e52c4188b16f2bd5ad0954c8f1" },
                { "oc", "d57074d9786c4c4c93f912e50b266b1e4beb89e7cfd8332df5618f66e3ff8171ff0cc00fba99f761eca686afa17b661fd33f261c5ea23275c81b81427a3cd46c" },
                { "pa-IN", "88524e45a3de8ca432e67e394261f9ff6d4adbe5ed65c2be9f7398e392f72b2a8cd1afa793c41c125b89855bf479610f6a1bebafc16151859977710875551737" },
                { "pl", "9b0a639962bc12fb76b1eeb94de84e8fe57e28cb0ee5da379dd12d979d561a236d2a10af44d41528664113e619f94c5ba3f070ff029ec08ddc9190ecba5c9a27" },
                { "pt-BR", "162a14f856e0d4ed562af5a196e3d2ac7c81e8e293e2b1d2dc0a7f605e3d3149ad91f06b35ccc799a06651d24c221c7c278e3b26333bce384d6298de14c20e29" },
                { "pt-PT", "a2ebb803e7dfb831772b24598dc80dcf62a8705daaf939ef147a16483f592603e0e678d02d58556766d8e703fa1e8cfc98f2dc738f78cb93ea0cd72f088bd976" },
                { "rm", "840a8c0058e2d5c79d90956e160c4a221dfceca62718738f73118105dbbd50984caaeb3dc530728cdf9050a79f9ce1747edf8fbca85b5e2a3bc1cf78c126c5b9" },
                { "ro", "2c7492245d37a0bcaf9d760c38bbbc9a726a91cd209737d2702360a1bb42e005904da6ff7b2655098e4139993c1a4e42fad3f10fa73a01c90826fdf19d2b722b" },
                { "ru", "1f6812bd078e4bec753c6a2561e30948e4b39e38a2697626ac23c76084a48ae88b933e441ba3ec465139e96f9d76ae5cf271a017cb1a7c7b5439825670c95c83" },
                { "sat", "75a4bf2c345f6f6a8341052d503a9d1d1c17d20e4c073a7ea3e6c41217db3b35aaecdf1e7c43bb4741290cc97c74e057f45f13356255035e0de0f8c9ca73bcdc" },
                { "sc", "7f42d9829c3c5c1a892a53e394420ca4fb4cdcced39884b40d139fadef173ba20b0c9c87c10f27adfcdafdc0b439fc1cd89c3a28123865455223b485c91f19cd" },
                { "sco", "979d13bae1e735993154a01cb95e1215fe24ae7c4be920e840ca4a874e7a3d63c2970e9bf8c913aee68c8ad6a4b37bcc01fb715c459966376fbd28e835ff13b1" },
                { "si", "4cb7511a3b9b91880ff544abeb7e1efa694f0f6082123e34528d77168bdc4301fcd23b1efaf21fe428f31e97cdec4e96236bce922a8c5cb9bf7fb60911491bad" },
                { "sk", "a77a00f301513f7f67881e5d6fef4ed7c1b50b3cff61b0fd0117276fdee98e8a8dd97c9dd1c84fed6950501724dbed7cfba7971fdd6a342a18b21e06bd79b0be" },
                { "sl", "e5df3b41a65b06a3e5d199038a555782049ef7d947bc18bdb59c50ffd12b629830065e0f527fd0912dc34af656e773a0178b336f6eba736b058d710bc17f627a" },
                { "son", "5442d0052944f0b7a38fde2173d9b5d0379fef223e852ffbbdfca300bace744e3a5a2bd078a6d25ed5ead42b7f7b71877b9d17b0d29dc1d04b902391607b0e79" },
                { "sq", "3cadae87c520c8e7138b9f5d5ed2e1e7e5b6e8f6b3765e13998637a7cd6fd39ebacca64f9c58c2cc08831549ca2f389a89347cc3692b61045ced23bd822563ff" },
                { "sr", "4484325011c900ab8452b4c1f233861c3dc4094ad385eba755a35b3fce9318ca31dd7c6c1e3879ae964c6f57754407daaa9da36c4ab467a99c2b7db46fe6a355" },
                { "sv-SE", "1381b20cf4babd826fae6c89908d61e1f399800e7c002f29c3abecd815dc659e9a4904c5d7a92fde78cd4cfbb84420f838451d22e1112a5df63070f6f0ad607e" },
                { "szl", "121c7396690dda5b98a48c8dd759c11db69bd2b0a110a8b709106b44ee1c1c769f0dd30cbfae0f94a3fb9e0bed55289541e501e686d0870f2a7eb57cf8b032a4" },
                { "ta", "4ae213306357c181327372d4a97e8f406c05ee909c7099efaa9996a1f1c5ed072995bd453cace0fe90fdfbbb17d232f93f18db18293610fd51ac2ed2f91fba84" },
                { "te", "f590b06de8bb693b38b6e505377b5380fefda083a924de71dbba89576ddb36ac414080934d44bbfdeb6ba985a8ca80847363380f20f66d287a321366bf4b00c9" },
                { "tg", "c17390a631868318c134f01f374c5d0fd9ce316360d50cb7f87412acff85619759e70de5e0bddfa598e6dc0b795255b67be138cee9232b8753a218f91c8c0b0c" },
                { "th", "653cc1293a528dd896ed6d84a58a74e8ae38d4961c9c20f8f84131c279478ea6e40e37fb0d7a57d714ece7d5438729b95118c8396ef1f335cc57a035c83d9bb0" },
                { "tl", "eb9f3696d00d0d7d928bc8ec80e699e19926cb370bb2be8d4f0d95e7a55d80432d7e8787e35ab2108b996b745bc6697289612ef719d34cbd8357b448492b8faa" },
                { "tr", "69b28448edd245136010b3646da831cf950645b3a9d3714acab33b54fd66c4e91883b328e96f70b1b9be28b14f2f62bca5e02067f1ba304abc565ae55561d9f6" },
                { "trs", "dd8eb4b4b2f7e201142e3fdbf4ae1f1edbf1c19bd7940ff948c1ad61da45afdd84fc6c630ab06a9b1e53af3b9ba0274add1a36de9311b9ae5519f9dbb9d3513f" },
                { "uk", "c93f0732def51b3cea588cfcf20da8e36cef3c5f064f1c6a2654c93416f30167a86ef62a195aee0336ed9473fbe8a10a1f530f21b75a0ed7dc05190c43ddc9b3" },
                { "ur", "fab330d8a37a5af5ba2fa46223cabb69b5b21e58d808f1cb12be53e60b32710733250974bd062760ede88217bf7d5b22dadb9b0d86982d11081a5b6b2318a748" },
                { "uz", "b60856ea3f02eb0a1b67afbe77510defc4055877e35ba423709b288b55c5a451e9edf1f73b1ce38affa665537d25284b56bb5b7fbf0d1187bb4b642b4a93cd2e" },
                { "vi", "da83a7513112990c9c4fa240cf3f013175f6aec3d1ae2633f6a4d558b0275e6e8dedfd993c0941bba9de2454f4191e5f8743788d65f8eac070eb1ff86b8c7b35" },
                { "xh", "319e694a48dda27de7e61a3488d6af3c6d6ec841c0f8c9c65fc246825af7c145ef22073c3cefb94b238944acb834593194769f35e086694205911d1cd12f9f34" },
                { "zh-CN", "696510e81408e73f28f388efce39c5541716de867ca3b82a04806989c19e5cdc432021032d32dc75eb95cff4b53796b1e205fe35ef5113f69dfbd54f64b3f8a4" },
                { "zh-TW", "fea31d7eb772e2fbe6095279a336e639fba899e1eaa80b6463af0395bf293288e0e22539a56d3c848a304c968eb3bb57b01d45f794ddecc46237231c858cd5c8" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public static string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
