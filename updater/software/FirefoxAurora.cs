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
        private const string currentVersion = "111.0b8";

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
            // https://ftp.mozilla.org/pub/devedition/releases/111.0b8/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "a3403a86516075fc0f48417c9df0b4782e795954fe4e01d8f72a61828363ac1d781a83eb63cffc3ca9eccd4b132fbf64a4011ff951aed54ab61d9c33dbccf6c5" },
                { "af", "a2e9e3b7fa64c153f904e328818f886ca69fe914e7d7fe38fc0073ea5769c74f1b5a26aa9246ea460b76d155c8b0e29e156d77dec167e8f1ec0357c32bd0502c" },
                { "an", "82f535818524b555345c330aef419ed97d45f28b0e60d86fb9e972a69bcdeb32d5a7455c268a17a6ebbb897215e5cfc56ae0c4e1b702753fdfd6aced1a3322c6" },
                { "ar", "33758bf8d129de0019b22e252e265b2e7d6d2db7bd1d00c651adf7fe74372ebc8205a5c7f0f3ad7d3a45a503d9d74f853bab361fd9b53556bfe556fed8fc3733" },
                { "ast", "ce6ab8628db7b2efa130757174070135f133f5a67c34bccbc99991ead66e3156e122fa49f69760f5e0c3be2acad3fcc85f717b3377ca9e03cbecb1fd11444e88" },
                { "az", "08002d3dfffd5dfe5d4a9d0c43e7666314a2a1c0d12393e0bfb4484368d539b2c6a5d19287fa3b86203fc5f9569b211a02537cb4c76742da21697cf8e116140e" },
                { "be", "2773722bacf0e6aa269d436b064551ebf41b0620ccd163fb913de88a9b27de9afda238094344293dd7b805726ced4ff01ae22ceb8b13809721cce9e45deaf988" },
                { "bg", "a6d2714e6cb9a7d66ecda9d91bee8db8ad9f501254a51c3fa01aef9de42cbed13fab354cf7fd61d75b79c2c9686b47d56e1aad78f064609e954e26c5a688d71f" },
                { "bn", "487d75c281775f0de895288ebaebe06bc2e8c323a30531bef7ed0afb859d1fc5049a27636462d2b0240b9119a05bd18e51b09e08b09234bfd216b5bf5d45f858" },
                { "br", "b6d27a6286e5dc97c5885b160b9d200bce32f126388bb2839353f6519c003d103b1b5eb5873f9c20c5359f0a10ef077b59b872521839e23e7a28dda5c29be52a" },
                { "bs", "b5a87656e76f2e173666a18017c6763840d1a1adac840cdc27d48017ec13a14cc6aa9a97916f723a8a72ed0e7dc775d478ad61d6d4e51ef8b773f186c671948a" },
                { "ca", "06c02637c040968f4abe360a7689825abc27fa0887fe202e40df3c138fb6431d4245d183f3186231228d01fd613efaa0120964cbaeccb161d0cc3a6466eb1ff0" },
                { "cak", "01b67ebe028f191415e9a987a10aa63b25630bf9eecd2dc4da2493ae83b80bfcb9bc6a51189521bfcbad070d19976cb5ac535ec8a4ed02aff42e4860bdcaed2c" },
                { "cs", "7d81ca531de895cf5d7ee80339af1a4719de317986d64ee023a60e04305f6c5cb571ad1a88d61e50f57eba76a9f1e1939a9ea5d9601f934cab67b61ca80c0a77" },
                { "cy", "61eb18cd3b8ee560bbabdf617e51a7fa788614bff096eaaabfa508836b8a9ebf125a4379bbfd138a8511562e536d59d0190c6de54a36f301d74e505bd562a050" },
                { "da", "0e86eb7fa54138aee125538fce17743a125182e6702ada2bf08759da2a0385febdb6ae7d06b99df39d9b9f170cc2dc753413de5e07f9a525fec674e69bf55358" },
                { "de", "c3b1ba24dcbf8692ec544d5a306cb478a709e51e996f276e6c3128d6978ce4964cd750406e1f1517b6a5cab5c8508df60016a683b592ac844ba6f275bedf2015" },
                { "dsb", "a51df8a5f66df107b4ffb7b756f8d24b923c576f19f4d35d1edc101170690a22640b09448ead69136aace52064495483c5e3d663702e95e49abb8129b809cfe8" },
                { "el", "acf4174db1c1041c6240fe20527e69a2b2251d5a482bb0cf13a15cbf4f1d888c7300b760a581291bdc400ec277e353125d2d3a4a28a1a08f0e8a3758ae245ae5" },
                { "en-CA", "982edafa2e641d9134366b5b91650a7465414ec3317427fd6841a667526a6bf93f2747489c34ea5dd2a5be801650dd99c3723baa9837e9b3a18bb6153b788f46" },
                { "en-GB", "27c0b7bbc61c0c706b4793cff543933b7ceb3db78e6a9d05f7717454999c9749cb0709d403adcaa88b42ba5993d3ac21623db96f8c31439cfc9ebb7eb107da77" },
                { "en-US", "094608be125323ad8eb8b49c3bcc2c7aa9a5a6c3822e0de5f63fcfd3ead05389a35ea0991b0bc5d3c105ca38f29ead5ffcad45af306cf9d628b82f68c0539254" },
                { "eo", "4af34319b02ab0fc647c33614431b61b0189252ee2484e8564f6465a9ffd17f4e78e7c75444bf12bd9e0df4eb4dc031f7516f26842c2e747935f486e41780436" },
                { "es-AR", "fac67ab2913202c0c117264eb5c27f9cf7e520006598f4b88ff8ce940bef1e7c98462c585ab622a6222c981554178301b561c7a73077e0244ce495ebbfb79d3c" },
                { "es-CL", "42e6f86b1dbf109c5919591e16761ef29a79527015fd066f59dd4946d0003c80edb0b042e51c6adb554dca059b7e2d5c669d653dadb75230fc1057915ec7dbd5" },
                { "es-ES", "b8e706a49e30a75927ae4d1f784f34ca867dde357080b79a72119817af14d6e5bf9794072049520f0610d8f58c2d174a576c8a83d43b10cbfd5866fb37d1216d" },
                { "es-MX", "8a60a808decea5757273a1550b8f24ddc02424d2069beb5ec1fbe355b8c0319f8631722d2245526492fe4f97aff3a3698a47cedd985ce534ade93efe1433e5dc" },
                { "et", "9515a06178d9aab07dc18371e825fc86d623dcaada92f56e962163ea71b1f9928e5715aa0cb111e4a24473f29d13c9394933fe71b70a498bcc17446a6bbb9277" },
                { "eu", "06bfeaadae0d8cb2a659844011a82a7d1b2d995ad50e868fdad342fad721d6a69f1f58ad10e4ec0da5b439febe115b591b84348af72d4c05765d57bd7a2d1c6f" },
                { "fa", "d4dc7da1d0694bf783caf80cefe7da94819b1a29409510f475d88bd616405c07e68a2f6f8fd3e1d3559823e76dcad654439e21e5619f924ef3d6b7ff7e1cffe9" },
                { "ff", "2d44cfcd810843a45d8932d928cf81bd16899e17c195be6e18f6cc30bc7e81ab06a76f2536f848edc89fab8e628baa19b0f4c812b9e73c851a3436a30e883aab" },
                { "fi", "05f16c3fb8cdf10a2cbf767b76e37a7cea8082c930e9f5995ab6a03cc3e8a1281bb4aa9fdf0f318505b6e43401d7f21f6e7537d7354c7e67d428d6e819afd4e0" },
                { "fr", "bdf3f4eb62703bdec6bf52c74b7985f491f141a40844059378befcebf35da26afc93d2c7ddca1c99d0ed022270d6e2f14f1633ce2b41339216f9d4cb806675d3" },
                { "fur", "77a00f70ab6769244fec5c04b1ced25757268779090d008ecb175f9bdb47e1c28311e4823a5eb4d0127057acb50fc0603a12ba70b0c05d6989d0dc4a23f925e8" },
                { "fy-NL", "a88af281b1d6b588ef3d2a1a65176c0c11a335b6e9273efad8f50c49ff01b7684a2a6f488f7f95983f87a8393d685df10519396c090b4b0efe9752c26f8d0bd4" },
                { "ga-IE", "e926a8d676a4e5ec3cc5101ca49701af594045b915e33cbe3ccb76315322adddfefd8d44b1fd5944d89b9e976a9eaff97bcd4e08d853b91fd960071c7e72a93c" },
                { "gd", "c6b3c2b0661cc67d97928c9462482bab30074b589a75471670880a19c93fe83912142fc507ef0cc58e00a33f7a832c1159ddd63b43fd85a16bed06d25b5800df" },
                { "gl", "3f779ff83257fdc5ba46da3458bbfbc88a8a86e9ffede7401ae6ce6d5c8be01b2dba060036f830cf59854e69eb8ebe40e54480f7c6f922f57ca92bceb5d93db1" },
                { "gn", "7efe0d7c469847fa0b3644a3e04d5f6e3d4ebcf8d02766eb2733e14a4234caeaf500c3bdb8b59db741641c23a2c0de9e3e7ae938481e1296bea88b8964b7cb3b" },
                { "gu-IN", "70faad9cc2bb1c0cc60e7d54400b281f4ebcd869c46044ce1cfadb9ddd32ee7aa59309e31ca83e6278cedc707054a32f7a1006cf6032df744aa89e483303c866" },
                { "he", "397643d283f375ea6d31da06150af635a4d2a8aa51337cd49f0a1ff1fe7de45fdcb408f609893cede6df07c379200510cc35d2be633fcded3ea3163c27e0ab65" },
                { "hi-IN", "cb3e881edf9b95569bfb5ad91124360e17345f9675fa08b78a176b433a4a4b15cabd9dec0b12412a9d8fec558c5c8e78d9443934236ca37d2a6fc827e0f25a35" },
                { "hr", "a3282d3670b0c9bfbec20f9a1daa75d81b1c167a5c285e2bf32c8b6fa9a7bf031f9ce202532e7d3fe3cddd1437078869dc5ef1afcce8eed2a970b4c8ee934f58" },
                { "hsb", "3cd02114c8e1ce550ea107520f2477fa416a3da67b4036ae2b8362a45c1e47ed0e3c278aba45d61429feed578f787d40c6e74aadba324620306d67e8c91454cd" },
                { "hu", "aeafbc189e0fe7a1060c805a6f7f3f5be09b55840309465657a96cd8d47efce75e669971b0c69e50a4178ace41ace0f1276f381bb42f31da2e90cd9646216e9c" },
                { "hy-AM", "04b6dd9d851e1ebcb31b50833be1dd32f04861fa6dc7981291bec38ffcac408cd9b50c0340c9ca347969f05cfe3c5fabe545be59921a8307cf854f2e83a47247" },
                { "ia", "368b3138473c87a918d19377cf839c91ad348c03797c61a416069dc0bc641605e3d177303fb278a1398b9b6bb33a4edb6318ee48e42febd317d327bf8ff0c173" },
                { "id", "99ea94cd36ed42d62119f67118dface3a0fa2553515c1fe8ee5cc445c1ad006231bcfede16df829b7c1e3d02a37359854f3137e15db0fed8c4f1479b12215657" },
                { "is", "4dc0e3b375109fb97f5769f8f1fd1cae31ee24ac8efd4cc5259f1931ef736c65645274adaa72317939739eda27418897c2cf8326b48db9274403a0bc831f9fbc" },
                { "it", "c8761c57af7d8f83ef099294c11b865d7220c65a3f57f74c049153a2ec0868e807d5bd40465efcf999a07d512651cae04ecc70446e216d80b2003e21750e1793" },
                { "ja", "a5c465e9b6464cce5a9e676b4aa9e0ebd252b8f4001e9852c9fd4b395c24ad36d0f10996091537c0668b41190a83967175a6afc0df721fd2bb71f7815c5f8ff8" },
                { "ka", "c0ada2fe832386133b02810d70288249d55ef428de0be6ea65c3cbc6fd7d531b305152ce8b0e19409e733eaa39afa0b95968a95bf41d3961522e46f2233e63f2" },
                { "kab", "94bbbcc27d751ef6815692a676db63b52d6fdcfaef5b9fe667dc366ee9acb41530472cbf6dcf0757963c2a5c6d5fdba8416126e167b08d72c0f5226a4d72ad50" },
                { "kk", "e703bbdb608a0afd48ea27406f451f1a32341f4fd5ca94a55340a31bdb8e6e1c1631d8357603ff4b042b373249324fbe500e8838cff6cd292ff422a292c19ad6" },
                { "km", "54cab2c04cc6997feda857ce89ab50d49c88d7a374e9b4ff6b1005f643edbae1d373fa7ef8b975f40a97777c806ea9780421ea12ee00e7cf9734feda642daae4" },
                { "kn", "3c7926a1ad938a9dbc56eaa013c43b21ba20c9240394f397f8d170c9063fdee166c7208f95237f9299598d7ed03351237692b1614d31a5b4ad3e49e5a7d79430" },
                { "ko", "44af9732bde2e391795b957de00986f1b78c86c275805e5acb4c37aa47a07cc2ba3013d8cc3c1d05819a953f4e30ef7e2b7f41b769a7ab05874f0f4781e7e7c0" },
                { "lij", "efa55ec456bdc239d98c95fd575db7fedab07118dc71f43bbe93115c9c81034e1f58b3361bace599756173b2d0b4dc4930278e479d4b7252e7335fe4402329e8" },
                { "lt", "b187cfca0f9769ff494e992835a72384ef8bd939d9eaa01143fc7a3231fc0bd593186e9c020c8f3672b28e3f542258099c32455f1550eb08b33325ebee3e2767" },
                { "lv", "4bf7298aa1d5e1881cb4fa9a430632c6e5b2725775d3c790886a2d1d08f9fef2cfb2f1d9ac3f3acf18e300346ec73e30bbb4319a3d0dc054d02dd03d8765f519" },
                { "mk", "81ffc6d6801eb2471c274ea42c8608fd19c42db5c8c22b8c2b1fe659a10e21375e23e23b1a7c3147d463bc3d5bc6632765c563460bdb1d161ece0f8dd2b0f4b2" },
                { "mr", "0fb274dd2656c0ff05c946a0cc9cc1de8b4cb4bcad3028678e206d432a9fb8f56600f8a397bd6cf8c9a3a816cc1d7173a4740f485faf0b080a8614ea6ede9ea3" },
                { "ms", "0e776ab2d463f69ecffd55294619e8321256ff72ef45beeba555b243eae93536d023bd2c105ee54744e5d42473499898599a5c2f319a655dea5db69883dac3d7" },
                { "my", "11fc016e51dbfcae803e9e03e1c651b3c9f5dffd8a38f91d570acd2ae294dced8445077da2c11d4072680bc59adcd846190187a7f6a429cbb39f00a941f75b83" },
                { "nb-NO", "7373d2fb3172ac2df8808d3cf44772610b3735fb1b758579829b6243e7b9640709a28391a7e2d3246fa14db2c5ec8ca868a2f8a99c2ca21475b61cef40973669" },
                { "ne-NP", "f6eb966c51a5dcddd3d309a3fc0bfa966f8cd91e298dae0c9d844c5f61bcad4197fdbfd4710672c5dbb0bfd815b2a28565d034f7420c6d957b51386ac0f2da0c" },
                { "nl", "aecc53afd8965de2f97b889360a98e951203035fbd673cabd0eb5d28617f95ea1ada2de3d7177349d3e41826f31a25d6d4a101289fdda961b088269cd590ae8a" },
                { "nn-NO", "9dbe81f9f35375dcaef99f1e15daba52d6509139c9ce079c4d540923342fef074ec96fd588e0f5df43881c198b71c0b927963defc7582f241946e9666212e0ab" },
                { "oc", "42ca65a032a0bbf0d6bd4f1043e2c83bf17bb289fe40d8cebd760241079c1a14d778a3ed8204d56df4f8d872a4ad5b610717070be419fdabcb3a4b521944a135" },
                { "pa-IN", "c7c4f24c27e121b18a18f0df50113385880924120b7c9f2398adba05282d94803085716cbd2f236b591348affdaa17c4fbfa823b4ea38ed95b78ea2262c279b2" },
                { "pl", "fb20d1bbd2c1efd4af01b21aaecbddc008cda5929940e8f128ad2b5bfadbe61785dfd8e3fbcf2b344d75edf03007e8f2ebba7791afb4183b9dfabe30bfb70f99" },
                { "pt-BR", "1d2d57954131aca16faf7c49bfbe0aa64efd716cda0158afebd6a95d0127100b9a337a4879495d895dff2fd93781c8881a584c092dca1e3c37557cfbe10f5a77" },
                { "pt-PT", "fa66e206e16a3a85bd80b96b87503e258347bafff2d050a372739e891b5db3b8731e5f3230ba5246440cec0e546e8d146a44d178c708853347374f25566cbd0e" },
                { "rm", "26c722b30be42b8e7732118f850da51a75057712c633d569cef9754dd70f65cfe7855dccf3594df659cd2f2bc68469f2a1a1f92af5b51d05f6abd38e7b225d8e" },
                { "ro", "5497b8cce1b01f4b064ad206283b2ff7e70b5629080f2037749b61012f29b7de2718a7cc1820b469ee91cc12800db16b4a29bf75629d4a10e31179950abaab43" },
                { "ru", "4af6a63d87da6a67df8060188574680f8975d2cd9af0737dc7c52b7f948d802308cf78bc8ca7d56538ff09d4858c028efecaa4c8f5cbc49fd512331786dd9563" },
                { "sc", "80959d921e6123e3f1e887657b5dcf354d096a0fe894523f8474f52e902106be68fd4765ad53720ee1b4c97fb2d309434dd5d738fbe885c5f90a709df54847de" },
                { "sco", "3a90840556d789f88b8d296ebf663020c0969045dd0f25b7028ef590ab4fcf8fe79de628a61bebb642cfb5fd8ee6dcf34f28d6cf77333bac4830eec33294e68a" },
                { "si", "1afcfe4815ff57feebf5dc581e0b6f729b716dd17d37364ee89827eb9fed645833663183582bcca7a831e15743c46f39745a218845c25c4e4cc059983651df75" },
                { "sk", "90cfb437612b86dae9f36215ddf2489f8d8b8f83265f9e05cac5127f220e4b4c6485065673eb0d759a3e7347e9b171d4e4a012722ccaf706b90c42cccd1531dc" },
                { "sl", "71eaf059adc4671825ce35754d49f7ce4d4e275c0232eaebdf7f9a701090816208a3d4ab2e7be0c56a3fd295ad5a1a4155c4d8f8990b1ea72df1753364a66289" },
                { "son", "f334209bd51730bb83734ad0f859033d64e9d7c44a2d817f57b675e2d2e6a3d9f0971f5197fe172a77dab4d7e34f2fb99a6dd66eec25070a5b9d78a64e24421a" },
                { "sq", "2dbe0691e65a61d58737c13409c9b6fefc999d8daacf0b1761f116d7526cda89e6a307573a0fa861f3e167ca3b73be975d6cfcc7c8e0287c0aa4b6348675f2b2" },
                { "sr", "79b080ff4c6b5d95f8ef10f8e8bc017465940b08b32fbcb82640b5dfe3233d92c6f6178da61f297ce93ba3ebdf3032a070c4d510d7fadaeb5cdc165e52196ebd" },
                { "sv-SE", "320502105649a490044bd8f1d94550626b07d66f0e57d605f642d3c76aa2859dcfa31053235e90f380b51be835c3e503afb0129300d1cda0ab10be6a0aa94916" },
                { "szl", "914155fca5e8f15fa0d3c827b9825cebdb0070f7f63ff8c8f8580c778d458878b1133bf6ce9f7d690853471a3f290135c7d053fbcacd733026c1a92320647fb3" },
                { "ta", "a1ead79c6768351733bc153d0a27663013b444c504e0bd34ad8c5758d8f10216c78d0d424af617650b1237806832670730a0703e409df5949000ee3338b1adaf" },
                { "te", "7b71fccd1ad68691691634a9b1d7d00c89801499381edafb54751f3ff9eb7dde7edad1330c05ee4bbcc54145e800520c1a36ad95a69f7a77cd2004086fa16b59" },
                { "th", "9ca4c5e82d70c3f1342aaefe3be281618a68aa34217670c003b1543c2f943d6200fa4575b2ab02df71395f41b33e591df75145cc965f763bb2f959cf57f4417d" },
                { "tl", "04da462db8ef5d42f3def1c8b4ff7c713cdbae81f21a07b33c9942f9b7a79557ed3f0dcb1205dd132d47908696ab3c04796892bb6cc6b04bc16c17b6fbc67bcc" },
                { "tr", "0d5f5a9e7165c5ef23c4d368aecb0cee607bba7e0f82cb386a06a293401c78789c3270fe2fca63c0496cb0d90b4b771c3227d4ff00244b58b70db4fab07506f1" },
                { "trs", "6cbc3621468681df760de4c644656b285aa85b59655ab025d4df239375631b626d7f0cf4a3e1596f4c9d30fb15740dc457656564c4388c54ad38f1970945957d" },
                { "uk", "2e68dfeaf0539fd41d35ff77d905c939af1396065cdca5da9cb03ccb1245fbdedfda7d655988bff4c39fd950b25dfd220ccbf7b10313a1372431e9ab0eecb546" },
                { "ur", "e0cf0ed49ca0367975f9224e9da4be1e5978f0530cda7e2fffc0b8b87600e9d961ea314bf26129ad429b490541117ab7288cd2229dfa70535ce1b09ad206619b" },
                { "uz", "2b7558317c810d097ef8da96af79b2c84e6268b3cbbf8a90418dfdda822c52f28230728cf2a72f99cebe56801dc9f840ee25cf9644b2fd70650e345a94b51445" },
                { "vi", "583f24173445f25324a873ac610f73d5f28ec140d4a3939df02a70a95acae00f33d6e2fd046018c533d8d6d431efa33e94baa4f706bb31cd67547e8aca510def" },
                { "xh", "2f1bc3133208f871c39b73634d0a1e6ac6d75fa302bc39db7313d39d9523edb89dfeb424aa890562b7ce14d50d4db0c9e999fb28b76416754daa22be3bd19f9b" },
                { "zh-CN", "9f5d513961540b806670951b70a1aeae529d84052c3dc315f7935014e124cad0162730409762c15a9101205ec9104e4449833a85be4092476657683664551646" },
                { "zh-TW", "70f15deef61ed669bf29d55fe415ada94f9c85aafc7d7bbbfa8c5cb1c469aa564c16309eaf14b5665c024175fb055b74aae47ae99aee6d248370cbe85f444e61" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/111.0b8/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "142c08d45b2fb6e33dffcd773c242655bdeae13d3e116ab69255f83dc381bd578f4c145a238267d5be8d14c557ca17aca42d88aaeaa1e5fe4a82202f2450d591" },
                { "af", "dcc83f926c3d213f3e45227c242c4f938b14e7a4705c52c58793d22f87d1e0719d3749e9ab9940442f9a2f1eb27e3cbd59696ee25ac5f2fe1a71a928cbe4f031" },
                { "an", "6a8e53c615f087742c3a4a0a6fe61ee0c538aa0afef27c030715dc2f3418510ea29cb48d79d63cef33e85cb38549795810627019b32daf4fcc01e22609965a93" },
                { "ar", "aeec6cd55e93298c52b39f278aae87a68187b8cb19600f16c83ea8679d00e92de700ae090a6dc365697ec80f5cb5b3c37e67329bbfc38dfa8d6178a555827a22" },
                { "ast", "7b544f1b2ce8c987d5ab107b24021126354ce8f56b715494894a9532458734e3e643ebdc843a80bbe5e08570561906d237e8063dab23446e77d5d468961eaf7d" },
                { "az", "62a7f96c5b66ce9d56bdd78fbe3c2a18bc728e9bb8bc7be115d949989e7dc0d56bcf3eccbe921eb00da8f7e5a2ab081682d2c7ae8816bdb1c4ef94d404d6d38e" },
                { "be", "d2cae8bdf4aaf7cbd3206e17c54cc1266f34a2d43e6154afdc947f119b165b287d37b2b7b37381c1da7f726f5e050e783d9e5a96da86ca1d88a30ca967afbb3b" },
                { "bg", "27695d317d42de3a21c9dd808d29fa3256f74fb9eb8ba95b82e62c1727524050dc5c910cd8e8c05797218761eb7c3d5baf4797e51fcd1219e312c0c042c8b53c" },
                { "bn", "4213f662bcb2d8e95b251b6766315d48a90c24d8db5fcb3e5c7f9fd1123fa07d6f78374f1641ae768828ed74e0d71b2cd18f2b0dbf802c05a5bab227713f2693" },
                { "br", "f73f778dec0dc66bcee08b03823af2f6b436fb77cfb3e08ee7f2426079e5abd9f90e8c9e140a3f90dc5de42facc29c5581e4ba988ce22643ea34cd6ab906dd42" },
                { "bs", "e8fa9912f75b0f78a4c0867a1e60ca491a5cbb5b6a62a10f84c994e443fcab8318883cd57bd6a9af05c08024f201b35fdadee00426e98b3dae73d01781e315e5" },
                { "ca", "8fa414b25c9e8ea1afc390a1e5e410fdb9a808b5814e33790a1178113f2503b79d09e60d43e0542eae0e70c0286eb5a600db500760000e1f4268c8de1777dd94" },
                { "cak", "96e98720aa85d10c71f734ce021cd949d662badf5bb80548cd7460414fd5aa645e518c0201837019d62a94691576abf08b1228edaa3097fcdb11a79569a25272" },
                { "cs", "1675fa0fd816269cf55844aff4944a50867e3277df1c31c4886800f6fb3d3a2cdc3f266bafffe001e09293f6243be5a1000aa34e3d474f0b345d177d53420726" },
                { "cy", "9fe3d75adbbb4e52fdb234058519d1d8ba5d54a6c48e80c1df038b984593ac4538766e879e32b3552e6ec0ff0d1d0f0c8b4c0023f83a41e98aa02906650afc2a" },
                { "da", "afbb8c1fa6bcc0b276a460d39898e4a870dfca2316d2ee9b09c68bd48b3ba8d99a870c4bc1fb043893ca669fc1666cdc5c670e7d30939b49ee572b18a803efda" },
                { "de", "11bd1ff82a9ac6f69894395b1edc2fdeb56115fb91afad8746b06563b94ee13128903df6fbdd1912a7c13cb22abe73665e5b5502c107d1d81a57a4edc41faf55" },
                { "dsb", "e28f1c0614e6eba0ea3faf3d4ba67d6de313fcf8dc4d52d4a953ac9f4305a0a051f700df10b9bb60e4e900c26f502083d2d5092e95eea5bb18d24461917aa0a8" },
                { "el", "d95817d01048aa7bb8cdf86470fcf872e8dce1b137079ff8ae5cd0fe8f3284f6cfa3cabeb7fa4d9f948430ed48a976a1dc357f0f50fc24979d42024dbdb8de03" },
                { "en-CA", "9a9d2495c32a0c3decedeb37d71e972d94505729354da933b9f6dce45b94c1751fbfd096e0e39468c7d2ff318c7ff6159996d1e12f1fbdb7290c47c5e0367a26" },
                { "en-GB", "a52a1ad275e9724d52f6a8d6d911a22fe48a195ecc36a7c1e097b2d3d83c762fd6bee9a10107aa240030c40cf0ce7715bcedf7a93369116c0529277cc11a68f1" },
                { "en-US", "3bfda2da76040fb1ec9cc74b5c41ece62a55f7fd262be4457e0d23c0fb32fbe381973fbcea8c470527238cd6e3be19f8b29b49f084d1ebdc169cfd2a515331a2" },
                { "eo", "2573cabff0f2f4ec0701ed12348240c73230f758bb9380b2346f42b5778d2d386d36d86094998d720fad5ed68786a186ef029f149c344ed6348335585c18d1c4" },
                { "es-AR", "5dfaccf6c1fd421874021b0ac523bcb7ffa751df2afa05e8d1417b030112e9043a70ba1b1d87c7cc71a99e237b0e5cd93bf5046edc4a320387b3a4079ee19ba8" },
                { "es-CL", "599660c7b20778efe43314db8a29b503e926a007e3d99b130f65bc3dc665641184e1b908cd7a96b979180c4f800b67b2f66be84ea4d358111f5e55ecca2db71c" },
                { "es-ES", "2f0a79351ccfab90cc116989e5668994f498dff8a6436e4a73a0a7e89ab51d2722c56f71bcb863e73b4bc8b423f860ce6991587433a9f5a740ca9d3ee74d4523" },
                { "es-MX", "f7d51d555c0766cbfc6874af4ba067d3c1a14edfb59a17879896603703f5e62c03630fe64b6886f217da7bb67049edb74f0cae626d7dac6cdb9a35ad5cb55de2" },
                { "et", "f2c7d58c6bd3600ee2ee5090282bf732c82e496d4b233b452bcaf85e9716923e3f151a5c085a26232189251993bdde1303fc0c7a08c83e778e6c07058c2b9580" },
                { "eu", "98e001089ea689ff176b1eac0a5a8013e7796224372b47955b673ff91519b4e6421337b56d64ea440725d67a4ef6184c0b5d600e3f0d4f3442c216e42a088394" },
                { "fa", "363a8638bfc9371c351720a8a8bc42214f5858b369788de709b8c5bf1e71fd9a6d0c9cab94e655c114f17071adae587b22e101f6d55715a249753afbdfa1f516" },
                { "ff", "e01d35c7e80193386f466e3decf1f4ff8b6476968f50f8daf8d00ed1c249f4acd29b32916cf67d900667006ff50d577786a424a95989579a4217eb41eb633bad" },
                { "fi", "99a906938cccfc2c90e77ccefca5b1276d4aca9946fe681545cd0c2dc0e8e4d1b4bb56f603bb6bf126d2f4ef14dc6ae226c871dd55388fea4af3aaedd5ea99aa" },
                { "fr", "b09dd9a485c986a9d7e16c1ea0004dcf1eb3f1387d70d6d1be6ac7683d7979417b76339cd9ccdac1e72c40071bd0ae71730efaa428795001a604bdfc8f1d029d" },
                { "fur", "693c830c786df47c0bef99c1c3428d9c6c6a792aefe01246dfa33295445a4dbf327235ef4823c495dc3edc58b082f02861ae949ff6be4e5155926eb7015693db" },
                { "fy-NL", "dc0407cb0df1b1dc2228e2b5cf2b5f1ab5f0b86f1f14faae2872ea2c8c6fb28d999dc4b6bfba0402dad94b3cd598933fd0bb104937324ff017dc09969d53c9ef" },
                { "ga-IE", "b901551bf9e7d3ad69e8d1f00e0003b6891bbfcc4a2063d0e6af354050a4da767bf52611a2f7b2ee0b632b87124085cb1adfafea5d8d7d85f853900a7eaa900a" },
                { "gd", "c6b0a54e40711bb63a957a73cccc8b32cf5f1f7e2546989b91f6c23749424fb0f4f177e95a9f7927f21a396d56751c239a2d8fce1870e02ce6fd7f2c61b800e2" },
                { "gl", "c544a58f39a8a75f9b96eae87ebc5f2006b0fb3d2f67d9d49e8b60dd5da98de387e0fc4dd2516ddf5b2c188398c53ed272fea8e4b4d085ceaed795b0bca7b4b1" },
                { "gn", "c1c45a943798cbca420ba4a36e0517353774f152ee5602ab11dbada5394c700ad8ffcef18ebab27f24a1e6fb7f667ca72cc61442aa128a91ab66b6e47686cef3" },
                { "gu-IN", "7fe4157ed9278f9592086fb558427b10a5ec720474bbab5271e569f1651c554eacf91ab8191b019a0221c26130142e864154f6f153c28a57d1356965142afc07" },
                { "he", "5ce706f77fc5449610e54486c531f9c1ddc87e2289dd28d1d1afeb1fea0960c80c6d5dcffce3fe7aa826666a8b2e3cc1eb8e28e8b94a090e81ef5eace47fa693" },
                { "hi-IN", "f42f4be95a8882ae2132da5483e3c430762aef5156a52ab3766f27b6ed38df7f2de619dbac921f80692c55070b235af1c540cb817ca3413988668f0cb1973164" },
                { "hr", "a74135433028287fe97a05cfe2eff6edc99e53feb6baea1b6111584037cd4ba243327d689e03ddd6a5438c56af1d2e0e84d414a181e4895790b4a998c043b02d" },
                { "hsb", "a63b439e514a6b11d5e07f13a0df37aa4eb6137b3f942b00ca86a8e597ffbeb4a74b9197e49ee618e5d099842027c8466830a36f1d241005ff23f2a4bd93f8f2" },
                { "hu", "c818df665eada56d164d73f3d80847c489d3816a64d099f5d18b39ccd0ee832857e4eee13b3792312fd3e013244373932a7c89e9306acc26da7fc854a424ec48" },
                { "hy-AM", "0bc3a20549591340aecc97b47f6212c25e5768a6f9bfe167fed0adff63ccccefa06f57c08d763360e9e23f92402a13c41c3311c8b86ac5dbedfca2c410304ca2" },
                { "ia", "7c42822c26c5b8e3decc9658808f21192a116c0e184a8586582ee83a452fd39438278881a9c7a3618065233425953d21792ff0fc7d71c42d11152a5ae6327c78" },
                { "id", "d1d1572e24600523c010aea8a6bfdcc81ab27bd23af041bd22e8b9933947fd0d9af9b1698685800911c039942573edb9976e2d59d8bf98b2a0687b1be0c0af7a" },
                { "is", "ebe02023241e785d9639971c10dd9da1e3497ef47a289e9da2732108f1b23f4b03c0793cb1ec32e5c549ed9379094399884c281f0a4d4e849118d1f2f4bc9c06" },
                { "it", "caf75ef81248860cb7a238846d12de84766ade44078efb19f5ab0dd2ef8af00b7e5d847307043ae645644c8912befe8ea5300b4ecf4d64ea42096f207166647e" },
                { "ja", "09feb510ad732ce63b468d00e3cb083fe2a718abfceb69df094cc9020f21d4e17bd09a15061e780d366c2bf276d5b514b518796dbf3734523d960692ce996674" },
                { "ka", "affe7f5f95bd8e03c58c92a438e771a8d20ec6f8f1c623ee558250b679499e48134a63f678d5216cba640c0e4ad8a4017c8142be2cd77182660edf75583b684e" },
                { "kab", "365bcfb26239ca3f13854958d1e23ebedccebd227d4aa7b93fd5aac2b29c2d3fed03ebaee93f6454b7460a61dc1fdec6b712975bf80a52fffdede72ebfba8045" },
                { "kk", "990b872429ad88ccc8780da395d5138e9a02396efde9c59a231d58474efd07b850b9ba867e5c5e667648e84e5af10fe8a19126c81509458b8c02217b37653bd8" },
                { "km", "830a699b136a77516d4b5c6a9eae63a2c1de8d620ef02b75630467979379c5420fe7531cd738183dc6637e09efc7cf5c671ce0898cf0e569957c82af04821d25" },
                { "kn", "9a0d4541d450e073cae83741da91d50a4b15636d0f108a3681b2b5198ccbdbfb9d3984a791ccfb177e5bdebb4bbe8d823c936fc923cff809056d6bc3aae0121e" },
                { "ko", "18473eb198598facde09be4907640800c9b4aa9de983e43ea9c1e98a1d2daf3dc58dd8cf00550c4067623067eff78b4cbad2b8137145f27dd951725b7f4f23c8" },
                { "lij", "ae57bc4fc7cac4c707c562b17aca54ef0174e498c6ba9e7480c2ba2143b88cdc9303e53a83bad61b5e1d3f1305a45156de85621c5a8bdd53fe916a89108ca351" },
                { "lt", "7faf8d77315084cff2b36db4d81c3e6c1139e99ed6ad4dfd557ca2e120ba72348ac7f6fd613929ea3378d6b7549ab3c8a1c329ecfdcfacc35d2fe96b64babe9e" },
                { "lv", "0e8362866647a4fff5ae6934bc3b7f6b1ca0c45c86e9d2267597abed09be3a5803fd61ee7023902b5a7a1c4f20c5cdee598c1b1931142a1180576fa91a26c6fa" },
                { "mk", "bd6e2737ddf9c46f0837eb93afd651cd74c5d9a6b3eda957af5ade3f34d443dc95846fdb3973a13c49a6dd006ee1dda27fbb3a42f1c94af9ae3b9855d1e57aca" },
                { "mr", "683568e0d89a86fa72b5bdbba07890f139e2fb4199356bcbf5d4fe74670297e7c4f4564f0e05a3671b035fc1159bcca827bf86896a4a603e90d27c92b320f77c" },
                { "ms", "1f2fb8776ccf78b5952ff21a0995fb2c0ef854f19bdc54ea06d7d14a67719e669997a523779fb56dad4d518267fc4454aa45d879d18f8dc7f9b04be485eb6e76" },
                { "my", "ce441a96bd7f4f47cf15276ce18f0822de08a4bc4e7530938c5ff4692ce8b2d60d4a571f4bf9786aa5ea564dd4026de4d26d6f365cd3158e56f80143573d3392" },
                { "nb-NO", "80c37ff9720504ee8526070b23f96c00e3f06418b460a78cc07fc295dbb71439c3a6d58ec736eab2f486e8792d8150d190a848eee9ab28a8bf9420a15d3bd6b5" },
                { "ne-NP", "ca2425364d2709358744c49722896f975e60d1502b51f07331b5fecc263bb36d79f9f1e30f389a516eee47c44ac0904d5344aaba867db7a1b7b8fb23e404a95b" },
                { "nl", "2f1e8dadd5fc6819d9c4985ef1fc4037f0e468b3a8deb12fcae745fd35ff38ed66fdab980593219ba3c899da039689b71989b1a9abbea5ce529f05fe0d63b015" },
                { "nn-NO", "901377cf87859405daa77c8d2e77cafbfbcbe09459b3ed38c07c4b8beb7ab92c7867c8b737ec190deeaf129227f2da0fef5173eaf1b4581bb693cee36fdcaddb" },
                { "oc", "7ad12bebaefe7882323bd39f37fbe647d4581820ca74772e6a808b06b399c663e58f178cb5890388db589948d6b29885336f30509832962fdd5a5e1b1aff20ed" },
                { "pa-IN", "bc1cafaa89dc433fdb8beca051d2e59c84b70bf88a808c24ade8148805e4c2bce74d3d00b892462106c3befd9bedd4298e092117d7d6ef4fedd3b83052b0cfe5" },
                { "pl", "e96ca5e5ffa23ec07c6e8f6adf156e0f8db906a744d108897a1e43042274384e96e80b1e46a191aa4fb791de61428231830e374bb9ebd3f5be8b990b73f91463" },
                { "pt-BR", "893b7d5f07d3bd3ac52beb0dab5aeeb21574acb9327ea0942cffedec2603887679c6a81e36993945cd5161798cd1f322a37f2ad065e36b2b1210d78d2e0ce664" },
                { "pt-PT", "aa762ae2ec8b83b0b2b5480c79aa2c3331b1e63eab8dcc83fb1879d851abd2eaab1d28a697d086d757ce405f5845963b3d5cb611f616e884701998dc77320cad" },
                { "rm", "4a4ff7d65e70c70cd3a47c28a7c9ff0566b8f96a460ccc7d644b09b3e167e4004922482d706cef1f1ce2e309212300b3f552b9ec53ddb919166b5de02f7bc2cf" },
                { "ro", "92b727e4c73f387b91175af481887772f22e09b6a333f1561f76e61fa570971956e9f471a0df950b26b5be9bab712252094b40668fadb10f78db278b9d34e4d4" },
                { "ru", "99dcee2b57ff849ee8374798c75d942ea3a3c70e5fbd03977f48cf594705046899a2579244f08db83132e2f8d86b7a839b6724deabceec6757260f1d90bdb185" },
                { "sc", "177d4a4502ca90c64eb8b71aed91f90c4993b4c42a90ee9a9cddb2af2ab68e99d8486938eed82983add7dd8dfb1cf5ffdf403d6c3cb6b2ba7462d24294ee2295" },
                { "sco", "f3b4afb38ff50c854792c34f4e9d2f5c50382cbf7cfca59a6bb6e590c83a9ff0c33063b183cd93aa982cce0ec226ffdf4ba31b03d6291728db7166413a8ced5b" },
                { "si", "15ff88d5c7c4d1362d9725eb883b96ec0715d006ee583160be3c22ee660ea4f3977369dad3545cbc582f344991763241cae8fa507f8eadacfb79780f9b3f39d4" },
                { "sk", "8e90532af2f195755a63b9ff46d7f376c514f0c825ef506cfe6157ca21c04099e3ec0f61e934bf528328b026cf776049e15e20ad697a8f93a0e77d86c0b8a8b2" },
                { "sl", "104424ec272d413c93b39478082af41286643318f7aa34d8db1127502f526e76aa389cec0cfa7c5786f6df2b4c288ba92ec6c7bd2653e8ab2288aff7fe604aa0" },
                { "son", "580a9897421a7b9503cafca0c6978392d21bb9ce69fcd6c8801300fc106278b799b9133c9a47e2e14df24ed889a38d351207cb435e5b95bc7392c9efb6c51cf2" },
                { "sq", "adfb31ebf76dd3672fa4e543f80deb09f6b487b005629cc4f07a2ee418e436a99e09159b4fff697c10649c3a9ada0da6329834a0c58eab84ba7e6a4a4c4a149f" },
                { "sr", "b00516b3b3919fe22d2f584b4d833fc82e7e6d12b5860db2453cf0335e44ca571599bb39f832c17ab9d6895e15db548cd13d8d398ee3f76d6f50a74ff1841454" },
                { "sv-SE", "02032f12a1ff7cce5fe8ba41dc3025eb9fdaadb409c167e64091c480fc81feb96a7c019d4ded9faecde6c1812eee81a798dedcfe68b3e09bf06a87897cc81e7f" },
                { "szl", "736e0828c34cd24e1e54386d76dd9173cb9a1bf03c56c4424e3438a2cbd6fb8861b702fbdcee863711ea42eb1864547ca7758a18ad44857e41eb2c6424fdbdc0" },
                { "ta", "09526f375e1ebb74802fccb609fb52bda40d8d0f302c2f6afabed2eb00ed3baca002f36a65948f3276c2daa4ce60f1769e11af275a90b39c8d851032c8dbb6fa" },
                { "te", "1908ed864c9b80e84800496bc9790e8c7d2bf8a953ccf75a2c9f7d88b0207949bee1faa6e45f7117bdb414df4b56678b92b32b78c2abd0aab393c17fc030ceb5" },
                { "th", "397da4eecbbb06877c3be613f932a2f876371244ecc30b0ad964a27cc3924252172d95d17dc7a017a2822bc517f243c21307d0f83ef1ca57bf0a60282e2739df" },
                { "tl", "7e7d081d42f67da081efa693674ea244102afce2c560a4a49563b828d2ae72423aae2e6758caee383589f6d23c052887ec9212213264c5ccbe2d30a1f790bc23" },
                { "tr", "01a3f0254ffa6634e0bff3ad0c41314247adde41ad9f84cd0a859302a93d4f535d69b616d4d1f89e4d989babfac82ee4c70091553b98b45491c6d8372f403f60" },
                { "trs", "07b0b287384e7426d801e96094683c8b11273be97f61271810493268b274390826949aa55be2781a1c4b2bf24371a5782d46f7c92f7690079fb31175a8893ea8" },
                { "uk", "45d7c0558212d9aa3314df2ea70154c3192775623f5a491615efc340b718909c95bce04dcdd27e36141440694a9d726a0bbdaf98b481479cc8ca94ac489092f2" },
                { "ur", "ed819807fcde62cbfd82c1b420036df3f4fb0367bc4415168c0a4aac373cce45d6400fc6485554ce59129d64c8234330cb628a889400365b39a43df399091e3c" },
                { "uz", "46e1dd387102c07c4885c46342ad697daa167641b2eecd1096bd8a55c7b35252d164e8f6ebd580e0d458e52f1835d707f9fd856f1464caf63ee87a81ec755547" },
                { "vi", "a458f2f125e235e69c836a1064af9f2a4b6e7d308aa991546de4ccfa67e3144b1a2ec14054be66f79f03f07997e7799a2d21ef467a26879fc2c288901a7ddb3b" },
                { "xh", "fe95993d9cd8e3212fc356f8a13a03575e0e53e93f17b80217c95f119760780b0c318e4418a473d8c70f89fd0e1b4c09217f243a015fc5971e420134fd0cadf8" },
                { "zh-CN", "9a4270ee46b339e52ebf3e62f114c4352fbd7794ced80924f00ecfe0d7d72222b1541b1dac1a8cd667aa8cb08e09471455df9cb8f2901ea0fe2c87b6e0216b71" },
                { "zh-TW", "d39aea7576913ae69e4dae8f5f4301df874a4919f0b17d9ece50e6b6df553a5b4240c35cc1daccaa88a4e04e42480691ffeb223de1f2625e0c6de83a2d9523a0" }
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
        public string determineNewestVersion()
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
