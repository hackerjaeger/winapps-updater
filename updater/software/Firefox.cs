﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022  Dirk Stolle

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
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/106.0.2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "cbecd2e50fcb7ee14c315c03484c0fa652137dc0c8a356e52af3bb0beb259ae4c27dfbf99d705e990d90045e06544583b5434148189224d000760e08111e126c" },
                { "af", "eef9243b642810f450ebc51db871b070f576c5cdeb8a5c8ce0c76564b6cb191cbc29c8641c8f19a602d7f6dfd4808e4361fdd17622cf8375a0c1b9ccc4801da6" },
                { "an", "bd201f67bbef762209754e47b553903c9c8a3b86d7e36bf4a369710e9b8313e68efeb9fda4e43d6093a7c9415a7d4eadd4c5ddce1b49f1a3e5a3613f66b8448d" },
                { "ar", "a7906b4a65d3c363fc1c3b8c9e7df065c17eb007e22d42150da1afd98959d730f717cb89db649a1960b44c0313509cc766954764d5ce79ddf721fd1612ed0e09" },
                { "ast", "67074d5ae80a58d5d501a519157093c57551c3a6eccce047410825120bb786a6fb22964877363002b7e734d7d61a3a1aff6d927882a7bd4082537039269ef6a8" },
                { "az", "f1ee559efa1b944544c16b102aa9fa35f871771226d384b285b0a83d0d3e3837effbfedc04fa44d1fc649582bf23d22c73ab96737e44636408244a7ca41e5fc5" },
                { "be", "3a1169548c10ea99089f79a33a53684c5fa4950e74657f272d00cabee6a66695d95d4887ad2247e83fe3df52d171ccecd0c61475d06e222a6addcfa9a3603df3" },
                { "bg", "6c1181cb4f8fabcc5ccc1acd07130bb9248c575ced172eb97bef3381cc91981f76f1eac866c733892ea8a136e1a004d8d2d7cbdb76f1c37f9e1ebecc4d1dcdfc" },
                { "bn", "4e6aab81924be169a879f5bb9d59ec687e344c0fc11a970e378fa7d6d19510c78a6e499db12410f0a1e569502df6f2887383a7737077bfe4e797ec162c8c6a29" },
                { "br", "0c4c58a20a1f9d41226d98d6f875017104071cc53b4bba34e3c0d86ddda0f613f9fc9cccb7e8915ce328db57a74c1979bc5de190b4b61e161ed16398e33c0c3a" },
                { "bs", "1ed29e46a40f15618dc7a177bd36744232f0ab9c747ca8baa981308dc769dbfca72c6d3e75f486967ce0f316288cb07a4e43cc7871f09d88459053feb61f9cc5" },
                { "ca", "4d68201a209aafc4a6439e22677ef311536ac50d29cbbe3fe6ece7af3a4913c82a532d3ba98c170beba18b42101273e321776a1a9107372bc0068b3ee5756017" },
                { "cak", "e479dfb2514c0301a9e09fe684984294588202202ec0a607e450da2497ef186a8b6647062c35b039aa85ff61ff9e637055c0bc0c5d91c80e9c56b3ecec3a9816" },
                { "cs", "243f717b3df3408b2b8b8b7b5148850b8448897a9cad195ff05e2d8e4ec1c65a17bc1d2aed8600518fc52b74842ba586661f99043155b9f6cbd2463ef86b490a" },
                { "cy", "f1b35afda03614fbeb9c6631e14e7ea57d7a1a0846674546d27c2e42196925a83c0293bab88cf27555b832a4799d4994fc4b37b332cf08de1899d28fec836163" },
                { "da", "a1dc7902fb36294084011bd7038126013b22672592f9a2cc7e455da86e1b534d9f32be4f6c78a69c6b84fd87e0f4ebff1547519d6a9a8edfcdaa1f02b6c7d191" },
                { "de", "f3b4839fba39bed01f4a7332b3cbeb35ce45aec1284072c95e1e8285cbc5b3f4eda5fe74a5960b0507cf07d5f6fd2c7e60b0f7e7b153b15cbf76006318b07fac" },
                { "dsb", "c8e657fe299ef8aa2e9e56fddf32deab2e0f8e9ff41d189d721dfe22aa7fe0500862abd558768790821f6853205f735cf9a7b84899fbc2344048c8b43bd61d66" },
                { "el", "a2a37badd49b3d14504901c3818dc2f675a413e774415889163554446c94d706ae19cc548f91959f158eab54587f5044ca56bfab0d0c9f87bbf2020b04aae76a" },
                { "en-CA", "4d3cb0f79d3d20ebb5ff1e0722337996578ef675c09c3e0c4de011aa3debf7c47df69788e756c9697c7330072964f6141d0b3076f7d62f042852dad103638785" },
                { "en-GB", "be53ad3f76f8c1e1eeeba2f59c45125d7da18a80a60bc958f90073cb077cc554c52bab5f1f4d1b1252f2a23f6283aca426daa34ab85953d9c7cab78e56f155aa" },
                { "en-US", "bb0b0cb0f1d1fbb5e2ddf439fafe70ef657b039da2bc71fc492c7d65998dd8bf22f325bf08a1ac284ee4529186d4126c03667b93d552566cf157a5a5db56d476" },
                { "eo", "712bbbb6bbdd94fb08bae4af5228a95816686540cf1b290907b6e8c273907af6eb4ddbb9e9c6c93d9f3332a288c6c7ffac6ea2901724e9771c92a429c074b933" },
                { "es-AR", "043735c03c2a9a0f6c118f3f4a9bf531eb5b049d5e7a0db429e5dbcd9eebbb700af902ad20a8a79a04992381e56b84da1beb242460d4e84f11c2a1e7952e8b9e" },
                { "es-CL", "9cac2cbbdf4ac0004c53ba2fd245f4aaaa334bfcbf6f44185fac299a6da3d55224b688cfcf263ee60c3f89b53c13f7ccc39886de67efb4cd9fcfbe6ea47c50f7" },
                { "es-ES", "d99e6be67d7463fab389c5af6d201a3c7a5220440f0d5ec12a5ad8cc5cd1034cfbba0c19012b3dd81ea52a52d4e9d751b5e1746154e2a637abdebbc5fc9c26fd" },
                { "es-MX", "c1cf5d07da8caf5f5d22000098194e10828823036f4c7a653acdcf399876720780fd932741370a5cd5a4392b2fc2aea115459a4d99bce80337f7618f8ca6d126" },
                { "et", "f8652bf6f6d77bf63d7a2a962c52b5dfdb02ab74ca442931e1025bc22d5979b1576e597f02629e2135a3d9712b17b93756e6a5d3a4746073a6a5407952acc930" },
                { "eu", "47873ccafa0a82f0457dac725eae2253479aab4331f012b230cbd58d9f34279e5398d66c9e82633a22d0fea043c73d37005890937edd646570070ef17119644d" },
                { "fa", "7c5f01d66f3194ee6913cd8d0dca110c7bb58e09f1f596ab1305221b7e4a95ab990354aab5d71d93c592474567a177c9bd67ebb0fc2d5764a33d05ba737d1986" },
                { "ff", "2ec7871f93c77c3aad1a0353b25716cbb456b69426997d6a3c34f02a1a1c8ca375a1ea8a37299300897d034844c79ab7a3555b0b3e4669c54c401100a53ebeec" },
                { "fi", "7f64d9bf7b3afb860ce95ad3b5cfb3ba78e619348a67a709143a114191860a9a23fecea0f8ffed465f03facde180c7d0c2c6bf5a8fa802327a5d0462e37e3c5f" },
                { "fr", "0141f2b1bb8a7a0ee5ba8960fbf7d8be6471e82cce13e5b9271d0e9da9fe517bf824f77dc6cacb795c073dc381bcd714a79afb6d1a43d429344287b5918f4ff7" },
                { "fy-NL", "07c7785ff8345b25580c9bb34f2a62dd8d8de6012a2c4c338cf23884d272dd32288a4bb50fbc8d4c35ca1e9ab57dca0dda9981d61a354a403b8b86f9dce28e58" },
                { "ga-IE", "db28d9a52d473ba82f018b1025ad0f31c6085831f1fb5e2638ebf3ebef0bc6386e35ceac2b23cdb30e856b4d1f2be05dee259862cce84c4d35f3cbfe29495bd2" },
                { "gd", "9c0cc7d091ec5d5ea969f1af8128f540634384952d6bcd6325294eb86321c0944e6939f453374d2aed351414b676f131b849521318f4d97877b6ac9d9120c5e0" },
                { "gl", "705252fed2e3c1e6e1e5e7f9f3c3a99b5d761422693324e874c506665b187922f8424822bcd7ba1c634bb73345404e5d6f13a101d22d4c26fb2630272ce5d24f" },
                { "gn", "94c55c12f5547d890c78be2ce5b3d6c8a81c4ddd44c52a4a1ba4baefe52442b187822d5c5c858c871efebf535eba8cbd55ae85a5504bb99634f296ac35904a06" },
                { "gu-IN", "9c5be20e1fd0a4a9012d17d617457759830c8e00cd8f2505fe51e5a8721ca7779c1d1513345e43ebd00c0e93cc10c8bd0ef6f533545014bb5827fe7470282e5a" },
                { "he", "a904f220f3ecafaafca97245366c39ffad5e41adf007618cceba4494fae8c792d3fba6c2f7049561cc55b9d794c2366f9bf51c130f3b87327cd9fc464ed3cd79" },
                { "hi-IN", "a612a6a6e8b12ff29e5b57e6535d9f4f49247028602c87c2392d8d4ac70f7690a4c226a6f09336beb9d0a1d9792939fdb06f62aa1d093c5fcacc759ef600bfc4" },
                { "hr", "98c87fe587ad8e74a88be8611197ba85ff86c199023e1ba0b478cf6aa2ce43e7f53dd07fe240b978aa53f095a3ac66618bff11e891e546a5f46e17c094d252b3" },
                { "hsb", "958a2d964bf5738fedb4d877cb598bc1340ffce3494d0f792f326be6c9b2195eb54b74b0c633116ae4932baa344cc6d098ce0880a6af5543d4551e93d01674a8" },
                { "hu", "2a634284decd0c83418a8d5660dc48a6eb48762eb3a22de058d0b4ad7307ca97780acba156e5fe96ada65f835afd12b9f8b658ef4e9a1e20b4d748aff3211a34" },
                { "hy-AM", "c1d6a252270ff62e087c570f3f91f95de9b2bfe67a10a2d59d3bced7d768a61051e932e5e5a8e1f38a7228e4003104dd0aed4c247a82a7fa62537a590556c07f" },
                { "ia", "c756f44bffa2f6e45698708b9d19dd394e51f808793ee4cd76f81406c5ee16c45fa980c67030e065d80bf7e3e3a472496d3e290272f2c9808cb6c8579f1970ff" },
                { "id", "04a9c6bd68ad543b764b227d4580d7571984ef90aecb8d67d5421a15710a8c64c2d4a3fbed55d21a4461dce6a1c0374e56255ef8f475cb50ed48dd35e1ae98f6" },
                { "is", "52ab0c4447a8d7bbe189b469494e149cc2a8d6bee78ee631e42b4cf98c4c76f3031daac3edc98fbd67bb314fee9bdd88febbc5ae377ea1ff0d7885a1fec219ea" },
                { "it", "72ec6e80b862a283c592db8e0a65552b77a8aae80bd6133be30d8a429384a843684d2782bb87223049ced82cba6dc1a19b5297d441b781cdd1853fdec7788192" },
                { "ja", "1a26ba28aae9ce71cc91cb3262f708b561866cfdbeae0b2e4c0d318e3c83520acf06deb450cc64e1e92a6ea5b3cf567a9fa3497d9ed1339ccae909cb2913ef61" },
                { "ka", "720d65dd7d0202f4670332729d51a9f46e4931e5843d23d1139a7a1b51ce28273a169292d796c70070491c467fa6ea7ce91b08f6bc284a01140668d6ab9b05e2" },
                { "kab", "6328e6482dd076ca166e5f96d7a551f7dcc314c68b1c04a956e672e4d5a72af66e292b07ac9fc51057547b2480e3766d915e4b3f09c628e6a313777629a2b830" },
                { "kk", "188142e0400ebe9485955c2f580f76b799f2ffb913c78dc9b2b0a0dc62ff878456988a5af7e69cfb1dc4a4befed7457c127f135385722af929e84b4f38161294" },
                { "km", "90b0cd90e0ec2aeeb1afb0bdab7e97de011390ab181d8857653b44a7d28bc5596a506d46b10d179ac88a023c9f960c40de3c49ba65ca9f4d7cbab101dffb1408" },
                { "kn", "73584180168df867bb26d6ae1d46a03b899a78cf226b9360367a084eb053823c5b11207ea27ba5b16ff315936fcb16bca5b192f5aee94e8f9aee10471f357d29" },
                { "ko", "37ed6fe456a5da7958eb860ce534c9993323dab783a6ff0bf3cf6b78267acce61be820631ec841feb3a85a52acc86baa36732ab46df93bb54aaad82a462e4050" },
                { "lij", "084e390bba6c3bcc555da7dd1e8736348b078520c203e6ed55287582d091a81b9038c49e6bed3e29d21386ae4d3de8f4e4ce2c44ddbd79a57e35582f8311e111" },
                { "lt", "eaf04eb113a4edbf3c940d8d4fd3836ba9eef20dc29b40b43e903c2b8361eac0aa3421dd484eff1fff59af549e5b49eca8f9f4c48e11e8a6ad146f6a43109ccc" },
                { "lv", "2f4246073bc8bb8ca584904bd1ad112f7a81c21e8403652b8d61e3c5c3a94168d5a33472cf7168b94c5110f81aab847578dff47f25b3b5b4b772e5af34468caf" },
                { "mk", "8bfd2b16f79fc035da90e01171bc936b2950ec73070a5f013f913fb6065d77f536e0d7cd0c42d2bb879d594029b9e70a49f08c57a9319fd955219f9c5b728956" },
                { "mr", "54a37fc4395c7e25af578e863a881a05a7b42633914c705b783a950472ca14a8aa113713b027175c17cc7363811fd015ad9e73b437c1de80e0516d0ddecf29b3" },
                { "ms", "cac28bc6bd4d5d98378e811d9cb282a223f9a20ad67279b0adf118eec2fd5b0fd2745f45aa1d8d27f9639b7368fd8e807e062a9f7ddf499139f7865881fd07cf" },
                { "my", "f2feac7e5afe08d2738db041fda7ddc2569dd4cb7983044000eaad0ffe6db1c33759723bd95ef7d4d31dadb27cadfd4d6e4fa81e184d40166525a7aeea37a55b" },
                { "nb-NO", "ea0da40f80e5a4986a3924cbc217f054b484ffda58673767d5de64e6e8bfe6d819a9b05cb8797edf5d171e653da26b0d097a4d7671c9323ecc6e5d4334838eff" },
                { "ne-NP", "32196949979a9f53ccb8fd9a743750d10599e4702ff718538f627dde6ebb456b066925a059063ef90e27e9a8d481b8b3a8c1aa80e1b73f257a98b1379cd4c07a" },
                { "nl", "8b1b8096cb572044488dd34c1285c9a5b4a5cc5c16549ad4f8a4e522cf91b002a969a7a3abe5872a147640b1ffbbb74a5188a65d355e3fe3364c2b2e5bfcb210" },
                { "nn-NO", "49743b5367504da0e92f16210d6faad466cd4703a413041061cb321ea139d89b4d87a7849694a1c3dcedd2da5d421ea00827cd7f453b7162f931427893db174b" },
                { "oc", "e9010a4d403aa9897bfbc05407e108d0722becd70f361e49dd9827af4dca4b2aa4d6195db628e4e2a425995f3dee9551a8ba759245b3993d73c46cc809ddf3b5" },
                { "pa-IN", "6fd105388dfab6904fa22a5c73570f7e10ffe95bd8a289405ccd92d2d73dc9c004292eeb52e65f62701ea7875eb44a601ca75016474019f895bf81fbbcbb1ecd" },
                { "pl", "b0bc6dfd86d60bc4302ff72ab4c1a01f6856a1fca85e543580e500f77a2ee5761568b6403c1489eabcab72ca96cfcf8ec18334dfe177eadf7a8c572d008ecaca" },
                { "pt-BR", "fea3e47a60560250b4d5d57788317df8b4aacc258a58ba5357f3cfb5d9926c18c73c26c7d9f9c22adea937bc65e8902f1622f3a20ebb78b7970879449f4d55e9" },
                { "pt-PT", "193d0fe6ce830f6ca4ffc1662e343fd3d644b11aec0f154c4bb7a22ba4fc643a3729bfbcc12d25e380befdd9980d2a7642c41cdcca3cf35988bdc89e0176425a" },
                { "rm", "569ae5429bcb7e6958a6242ce62fd88f64d6c6e3040af2f161942f9db4777ec92c61d798c2f3719647e001bd28826e05b0643e284af054b30962d29834200430" },
                { "ro", "60ada6cd81a4a095baa145801f6aaeef50bf2ad834c1b436b46ead34abe17f331b81ebb29d2d347a9275d874437d30d6aa137bfdd17cb1f182f44b0c2c7061a1" },
                { "ru", "2aa058810adfe2a986f77ad8c473d17180ba73a01d10284c2c432a7b75da4be172bda90a30ea4c247f2dc4933656113529b834e54a4bc9f3fb915bdbb541a010" },
                { "sco", "83b2abffddd02a2d555608ea2d2cb39e7ca28f0858ccb680d141871ce11aa48df25b341c937469e435d16e0424b80b20c6c111b52a0a4b0e59d3f80727621b59" },
                { "si", "873f838ae11b1bfd1e8e6152080154c683c1570d98f832e471af2bf5b2a296a1d3ad376c374a1902b96c4f72b1eda7b228f59a2f870ea4b6bfcebaa4885fa921" },
                { "sk", "1f9ca1ebfa4ee020b0386ef9b4e007961213cb54386bc76079b309e9bfc22ecfccd3e1b841c568065101e7403ef03db2192baf8ae3692a5d35c79a1e091805a5" },
                { "sl", "1b79697a26fdb23d5ae914bf3feb59e07fef9cc49612406efa0feeef4b8e5c412adab8449f7b890c1c9dad632b1dc774e182932748c0832404e3ceb0ee493562" },
                { "son", "21d5d501cd6d231c54d84be0677c759872a833bd166364946c33f1d2b9c5e4133598a70292192c3e19f244a084a8bb06d1f1940a82e13a1a19b1fb889e604c42" },
                { "sq", "0dc78a7d07e51f008c474a472e509edb0facc8d6f1e11d257e45f571c3edbefee238aa0f54c6c16dd6635c6dc665f8873f6b2d0bd87c4c9814187aaa44882efb" },
                { "sr", "87ae7f026911a6875e3616baec45d999bb882cd3e94ebb072c9bea49a3cc2520c3aef939e4b0d967f994ec294ee7bf56acb193c18a5ea72d99a2066ffbfa09e3" },
                { "sv-SE", "7c7e7dc43cff149b6282d9ef1482c419d379d90ca56e30cde30195dc82648095c96caceaf9af20389cfe42e620d2fb074bbe23e5c963752eeff633b991571adb" },
                { "szl", "ad094798c66d74295cf6cb3c8823c475f7a3f356468c4a7a0176e9a98e91e8cc595c846e7ad5d6e936077bcbce410383c676dfc81c98eb79e03234c6134d025a" },
                { "ta", "3c4d50c89157c7fdc736a9a3ce5fac6de63168f7d81e38c9cd0c65b2b47c8015d46c21a5afff2a7baa11c553cd13920549cc3b825e420793e93106a53ef489aa" },
                { "te", "a7fd06615d39fa0d85ff24fb9bb29c43c3298134f4a471a4364c4fa6a059b1ae26e0e10ddbc495ba48f7ddaf4d66962c4c4356e2fbc1fbe01b0211e45b48fe0f" },
                { "th", "0d6ed5c49ef4f7b299966cf9873c74cc29b56cff44e2eb565ef9cd0fb5e582cbb3fe263ddcb3962c3d1ab2142d9bcbce53c71d24b51df9eaaa45322f09175b1c" },
                { "tl", "119d549a6b9f597ccf0d2a413879907022c22d13b7e218137d7b6d91f55d37945fa2770562bd7275dbc20d5c7f30a28e2323fcde066e80cf4216cc54b8d91170" },
                { "tr", "e5bd4b61143aed5117be558fc971558552960982da93b8e56a31bcf2fc86955b0f1a1689b53f35d574ee8f4b8ab0ffb71c84bded148df5aa0d9993799a00fa61" },
                { "trs", "b241c431c81cdb2fcb9a273646c07579130a9317e91ce4654664c5eb8c926245aa2a8d3f1715cb0cabcb0e274ea7138c0cf48ac84673016a6493e347503bed5a" },
                { "uk", "271faf07e5d865cb9b246836fde347638efb22a4b1bcfc45d7d1ba2f4429006d47bf9ad9aa824f5187fa48e846e57acb9dcf3d211bed9af5dce17b4d85938a63" },
                { "ur", "3c11411015f1600978ff1dd148d9545353f92dcae36d99f99534726ded0677555c1d3c89023c35deda330749acc08706066e036cdf6e8e91da81b714d3590391" },
                { "uz", "419ba727dcc11a6ad81f9e7eae4c9612e5e0ff3cecd0dc0ee4d2af4b11e3f8f0dc13148423531c3af307cff0b8c0c557c38f42d514b5135f198844b3ab3c0c6c" },
                { "vi", "ae7108bfba90fb814948df97be4ff1c628fb0381244c158d3aa6c8c0d7714e4ba6ff26e038e33e856122d8a5b36f83396e40c4923ce196210002d8e8cad00962" },
                { "xh", "8744c685d8b66ea9a05ead6f5f00d766aa8058269f359a3965eaf259370e0f05993c640e3431ecf76d82d78c8525981a136ddcd298e5f30da01e223494cb16f2" },
                { "zh-CN", "e5ad24a2a30bf483ad569d8dc9dae8b991b6db8c8beea0e076c6d2261cef1daa51e87dd7870b18a84f88cbf434a9301454a27a19b6cfc12b46a3660d0f7ada5d" },
                { "zh-TW", "9d87e28fb671f28c6e95a7e4bd28c803a357431b4d945dbad8fe27e423237e8222b8dbca3d1d21b0f43bbe98c5c0e38b973153dcacc54ec50bd2581db571274f" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/106.0.2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "aa4a16af39a71116e234f7566c586705da7b07b975c44603dfbbc61cf3a85d23bf2562208054e8497b4697aa289192183c9cd2ea2c571629e0676c135506becf" },
                { "af", "79d0910bb5a474ea3668b873d0268e4cbc2f2abf953fde42e6c383e3a1ff238de3bc07e228eb45f97d2467f75c3beae51b9c1bd82c9f816589980eca82e36ccd" },
                { "an", "97b12d59089cf7e357dfd946b3906e469058c81130d1a6c9df9c9291bc7456e6dab77d019deea2bbda54906eb01c8523818f87c05b4b59b71b26913cb938f412" },
                { "ar", "8d14b4bcfe2e314d67712539aaf675c5d4157c6cda5e61447722b1372f9ba26e4810bd6d62ba21374f667a6d4f6da8283ccf14aa23ea0a4340f2b98378e55625" },
                { "ast", "eb340c0c8f8d11f2fca55864cbc9f28d6116c022fc2bb2e785f796b2dd2440644dbfce17e9e09033436185632e83186d15146485043ee76eac4fbb6d2b81787e" },
                { "az", "a083c2c5db6eb6ff4a350c6cbce2cb7b86f7750c8dfe71b6f6ccb971e2afc554769568b13d3c8e7081f467f7bf208d72006c1536b7297078d9330930bed3f38d" },
                { "be", "8cbdf4ea085291c9063346f4921cc05c08dbf9833fadc198e4f8914d19bc5948fabffdde2202bfe7ea1ff50a5fda9f194380fa6bfce915e337ea27cc694d708f" },
                { "bg", "fca32e2572f2508838a1ba4a42ea5ec45f0d2c99c92d023e2eb7757ea8accba5d67dc98795e4e2b12f02362114f35f323df545df70995b510b3b6dd3b1a4baea" },
                { "bn", "830ca520ab45d6823a02afaf1062d89a0e85c218fd0865fbded68c889a29fd61de6c255e7cde85da404a3f31029674248a54398c81e9d02a22309566b02c1789" },
                { "br", "d4d8021dde0859cb4106ad24995b40a9bf25c0069dfb720f42fc0977260926f27680e587069a69bcbe993d08ff58fff4c49f58663b2c2ad7e824e1b0e29f2999" },
                { "bs", "23f8cd20b6c7f8c7061b5684338a4a0875c0b44c501b5ba96cd333dd51d6dd392e03fceacc863460637b549c4fa054e5547aaafc7a95b08a3a3078074d30c32b" },
                { "ca", "066062f2d51f13b9e97f4ba098995d69e3370c45b43d1977023234c3118a1e63b7b1e0d70e48356408a10f0ad6cd5318af2e701e014cca0accbf4c4d2fe0b202" },
                { "cak", "ad0dc30683ce467a9a403d9936ffaaf2085365658184dac38f8d0867e7e047f9d886d353f947b66e4ff0882b055078b73654ed0bdf8b19fdc5024ead95b51998" },
                { "cs", "81c23726d9c3b4ce889568e52f36e55750813f77c58713c61d38d422956b74e37e0423e5c100cd7c1d2d426e7945a7b4bd94099e507569f287915edd200f6bc1" },
                { "cy", "d395cea2be6d2ad77660bf133a1031715a22348dfdd4e93b1ed5aa12a94c1171081746aafb3355082587db027da99243f30435873af3b8fc71d94798a7bb0b28" },
                { "da", "1e2ef0aa3c7d7ffd3f5c587505ace742b8ab174476f9511b601f73ca85944b168d105248135cc0463a3d43896defbab59244b130216ed28af6fcfb5dd050254f" },
                { "de", "8d63aa5d6f925e4a24ca249194e7c15d06e3700e721766c1a24795c007d25b6594095195ab596610252f0cf599249043e619f9dc1f0221d49e5352c85b04f096" },
                { "dsb", "d31a92fd311335647e440f5713aa4388e5cbe6c8786654bc67b02b95bcfad3c77944fe0f61f4d547c5c04ebba11fdb81add6b8cc85ae8a92e256bbff73d234d3" },
                { "el", "0522258e71a57b9b63df951873b36f649bd01d9d3eeb4e7a389ac54ed419b57436b944c4e7262fa4b0e7c37e142ec53361f1d80154b5ed71b69ed5c17e40a377" },
                { "en-CA", "0af7cf4a3b362047d9cc5b9401fac226ae6d95ab39b41ce2efe9bb9f0efd338228753da4764515270a0509cb1ff04b3e24a3b126e222db1adece092b7e259a96" },
                { "en-GB", "537ecbfda382c2c710169589d04fd8f271c5fa0874bdf8a45692d3c6953e81480aaeb0cabd9b3a051e0cc18367209cc16a80bb3f1459d0dc97f294542e448bbe" },
                { "en-US", "57b7f9c3542824796b84bcf27759c9bd1da3b1b6e1cf760d1f6052dcd6e1e0270dd3bcafe191adf9ed761a7dfbb28b2e71f96731b209e87f7ab73219effcfa5b" },
                { "eo", "5c9cbec1993a721d9e6a3c833fac768ed155991d50b9886d6f0b883eecb823d2d9cd2375fe08a3baddc45fa339514c61481d11b52fb2f82739e7d7185f337d8c" },
                { "es-AR", "4a6e0ab2e072fe7da288c3808b37153cf5e9df8f60d5bdbd339ca6754f9f75af547855aac3835a1214e775e5337bf0f73b9e8c70b75500b5397eb7b261b4c58d" },
                { "es-CL", "36892235667d9eca27c18a3546f9ff998e9baa54606462ccd3aff7eec768399b9e633f878942283ccb12b0b1f2207bff98554a5a78b606bf37cff16a38a055d4" },
                { "es-ES", "1d41e8da79b5ab3893d98d38d561df62e365c8d08db2189617b238bed94d39cb26279a3a2974c31968716b5705faf52284ecdc20cae4174141786e627e691623" },
                { "es-MX", "a4831a6ef44dab74487ca41163a5bd0178de299901fdcd7e538eb3fe2f767bf8cb9ec8f8b341b7ead854fd6957d91ee6623e0b91a9d4f24782c0831a8a15b305" },
                { "et", "fc0fc8327204071f2ba7764fda5e86d3c188365fdee4524c0c0f30b5ccd08ec90a2a167d4cadbf565baf6764f189e74d20495e12a1bef1d45038e374274aa3a7" },
                { "eu", "ee81cceae0f8b276892b4be1c255b1bd6ec27521127fc5678080b025a43809f89b211962e93e4f49dfa3c8c3af786259d9bde91456f108226e5619d6005677e3" },
                { "fa", "08f913c4d9247232e157df3a304a496dfc4114ed12d77af9cbb47b43dc35dc7bb2a93a6d4e373f616c76a3ecd898d6d4377b59cd41c2c3b9c2798666744b25aa" },
                { "ff", "7bc68b960634450cdb83b898133758434adaadebac5eb199df1dd233184e1bd9ab5d2be0e1016238598740d0f6dec390f3491bc3b13540e12044a703f420f96c" },
                { "fi", "2428960bfbd47ee5af46501c1be21f279101db3a3193f1530121f67c130f93f43d0ec394100be1422102c5d52da32eb01620f867fc1a25e4c3b861db331ccdc7" },
                { "fr", "049b9a959b982e123deeaae49c08351d0f83669357217a06417babf4aa808e176a15712b9910baf813c024777ddc3e238f741d02677e6c40ac324315ef99331a" },
                { "fy-NL", "fe946778f5faac1b0e4a9038f76c963e1ec1ebd4d6b59bfba152bd0d7ff9be5951ee37e369ae4f694baf0c88b6331d31f3fc3f887700e73ab5aec4e44ed98b74" },
                { "ga-IE", "1dfa29a78f4d5d0143e3ff2f498486ab92f5ae5b0c797fc2b9f1b73c18217ad15782b541e70941e1db39334e82a308179fd6f9629d9813bbdddfceb470ade097" },
                { "gd", "85f2fe7a7ac28845bca777e528a86df150687f1d89686b4c440bea8496d6b420b0756684d43326b1e8ce860bc44fca1f766baeb25d93ebad958403752d6d89f4" },
                { "gl", "7cc6615a7f8d8cc289481757cf03336718f97745014c21750c833e129a608ae3a46fb1562342830370b73856a8bfa654c853ec20c0b77725cce4d186f9ec912b" },
                { "gn", "08ec0497ef0f84564570c04f76e66f1c9be6167d0eed3a008d008b66bffa30253ff0bac2679c1c54a94e344533346344ffb5f345e0860e5a1021a3dabfce2627" },
                { "gu-IN", "d45ccbdb8e16f486b65f4b54850d4cae3edc220a7d3dd8416f89e728f45ab42f31a85749d41985d319289c252d6df3b3b31196996f6dc8ba5d4a1edc23f67dbf" },
                { "he", "cb80a6c08d556a0bf8ecba894a90ea18c7fcde8faa895ab3bd702eae3d5590f86d877598f2fb87658bb83484fcbc356873e827515dcdf87ec3db389b3c5fcfbd" },
                { "hi-IN", "c8d0034ceec94d7c2d518dd6c284fec5c409b5be483a5dc0a6c931619e56bdeabf92be81b624fbe27a5b5202b7fed27cb0e9269c0f870b73cce1e2085216c42c" },
                { "hr", "8c922a3c93642f9e86b7286e6279390c2e2662dbf2b2e06b74ef6c95a28eaa18cb1a3dfb387bfacc095c850c97c44191ed2a782ff16b2444a5e8e74c995a554e" },
                { "hsb", "a07568f3a66fb0f3aa8630e6309b72f4cc1286d3cc46254f6261fbda7abecaaf1e1575f01a043f097436d1729876cd6595c42e375064526e43b58c3f6c00961b" },
                { "hu", "b35a1555e336f2a59ff09d358673f7fe334a78def1b97fc3cd310b9ebebbbe350d73fc8de837e7dafb5e472a9ea7168e759a875454a39ee1fa43ad8ee4680757" },
                { "hy-AM", "04e9ebdf47abab42160611a0fa850bda7fbbad197bf3902923edcc51b6fa743799719149ee9d8b07e2aaec0f4477c160cd6eaae04a1d552d4c9751af246ca204" },
                { "ia", "194ce7042191b3793fe10d17c7a249a11448d93c98144c991c02d19da130541810929974b22b6c45b0e5846fadefcc14d909097f8a70887db43fa204192dd52d" },
                { "id", "7c155c9c49aa0e855c809fe1048051abf349c62e466cf198656e881bc5ce678db1641613050d42f7dbfac50267462a12b5af2a5cd2cf6b4f983b920df994eb6e" },
                { "is", "48e44f2fd4aa9c7b81deeab96bb243a6ebc1da79245a0106ea50f1eeb21ad2518bf70498085d07970447ccabb2969519cf2b71936f5df45e4983756f9b0e03e0" },
                { "it", "f2f57503402d72fabf267df1ce0be7cea4b5bf1e0a5dd5b7021781cd99916b868b25b1dddfaac249f9e3b03b659629f00e96ed05c2e325cba6fae56e573497cb" },
                { "ja", "42011024a77e31a2c81162f2e85447a1056fdaf76f51e8637c133c368ebd011deff411bedb5d4fae0e1da23663c6e86919c21839941e757be631c9117aa0d134" },
                { "ka", "faf0897e4e8c95ab8ec9c7e36f29ffea5f2762412f3352cb527c470880fcec223ca715d40449a0a623f0e62decc7831b2edf6d7a9d8043a86e8ac7b4b330391f" },
                { "kab", "6bb306d990b922dcf30ec178b2c96f32a861acd88a1d99db027b6e06f61f4161019334e697e416e583789f6d89fa696e0814be419ea2b71330e7a874c380724c" },
                { "kk", "ba13266a040e288f94edf0a26523e428030b6d1f2f25ef077d444ca08e7f733f3cb3eafdd01b7e094a8889a1ed55a02ab65eb96295e55499201942fad9b793b9" },
                { "km", "1ded946fda3ea8ddd10a8e522994fa4da052057a4832211c7863b70e650cfc62e2aad52e5514d6fe00195282080d9e083a998b8cc8b029c9cb48c98a89c57c85" },
                { "kn", "5597a80354ccd04a5da1b5bdf352a09dd44bab54c9bdbeccde5113a73895d72357bf5d178cf21fd9ddd22468bf36d34e436b6dbc73fc8177de21343c4913437c" },
                { "ko", "4838e98f192c667b20401d9cad2d718e9bc60a2dc3acf1ae850441475f5bb219ce0278cc374da56d1b6a1ea0cf83d7ccb11383e63db6bb077b7d0929b93121c8" },
                { "lij", "6199690fcc320e9ef3d6a59371178165d928761ff1465b2a8615759412a7f7fcb010eb1a4ef6cccd0b96220f651e64f05d109b3031db93ab42077cc3c10cdf1f" },
                { "lt", "d8d149fe0dbe45fbe0bd2c1bd107efc092d5c69d714df7bde74ddb0e23d6c68847fde35fc74ac836f109e8e67bb4c547f0916ea613187612a2f851158b05e03e" },
                { "lv", "e00fb0a0920f558a4b9b0134f9b2f3f40479289225807234ded49746e12a12f15190a6953e66b8651e09a7aa6450b353baa4c551566f201e300087d5ad49a844" },
                { "mk", "b7f2310c3302761227977e9b3a9f2f84991a6ba5bf2ec1efd90e7c36dd4ab4d913f0395d51d0a9c50672ea83ac8327b378a1e1665df499a0ab266436342a194e" },
                { "mr", "898a5f7df8157825e082f7ff2067074de2a2210b81e6516c32a47a9ac7ca144455bb59049a670f61ae7aaaace5e3efdae794e39e5d2955c1707d9fc8c8616ee8" },
                { "ms", "b745be491f46e7ea0f04a54d819daba713a45ed7f309eefe192bd89f117491675ce0ee7a51a22a058a8d357c428c37b541a320ce1e057312844f0faec6cd1abd" },
                { "my", "83c477c4c3e855b493aa0359fa5f038d3fafe376f9a7d3d7e6e57676d6806386e611674042ef5732237cd2e3763c390f8c4965de69d6292d094094baaa6355df" },
                { "nb-NO", "d220176a8959ddb96312a2949deab63b7009e53203d620cf25235a051fa6fd11c736c4ef1bfdc481e7357584248ff15b306056d9b711ea945ba18815512bca07" },
                { "ne-NP", "8c13baa33dbb17aa460932185ed4b8f76e6e6456432d945dcdc30c4d899ebc6568fdc1497bffbaf0ddf6e01f03e848d34180b1d1c68e18c55a356de72554868d" },
                { "nl", "16c6a7b31fa233f949c6ab567a594aebabd0ed082432d8294ca0155a93ecadfd0b1a8f1615272193b1371d459310bd557263353ea4dd068d48f97fa280beda05" },
                { "nn-NO", "ea02d7926977b5155e8468dcafb5d4718afd6eb6d8a27ac3844253eed6c73fd2a930bb6e9f745eff52b0f1c1785324058e623b18841cb9bebb6e8308b1419a3c" },
                { "oc", "7395e54d26c4e4cea8dbf6c756b01371e84a30aaa26b040ae9a191aefc148bfb16a9c3e38a8ef6fe523c84443938c288d3f71364e5af1caec7aa544d68bbfe8b" },
                { "pa-IN", "1fb6c408325bd0ddf05ca1b494b7a11b08b9eb78ddc9dc6e655507f407bf4a868932e09f4ef22196f30d5e16a696b0ea4201b588596648a723844c30667dfb4d" },
                { "pl", "4f13a72ae8f34995e5eb7662f579c055e21cce28d177f5d0851cb96aff4e6129d12cc7df5c7c205307e340c59798cb7f80475641e08df18ee6bf586db331925f" },
                { "pt-BR", "3ec2f56e050dcea789d98c81041a56d3977d0acd8e10afa34046c538941490132b934c724075146c78c199a837aae2bee41f40bff5ac3fe83a907077ee1d2fe3" },
                { "pt-PT", "bbc0f789383627f9bfe88ec03ec11967b78c708019822364574df06f019ab58f54faf92536efd6b6c5bb4eff95b87f2c7a242d41cf3843b130382d797aa7df64" },
                { "rm", "acf36d477fc3e326aa66b90e1e5caa5b76ef7defb1ad6e1ba58802179825de628d4febc16026a6c058c489b1bfdf41a9491babd53c2834c00377341d2dcdb777" },
                { "ro", "19d16bf4b73197e304bccf073a425f09e7b1782407e7dba4a08ebecd3760a221b3b0191eb97f885fbad773266d1a29caae150424340090eded77ad17a378125f" },
                { "ru", "793edd5f12cd2d63139838fbb39d5e7df32ae8a26b336efce128c4e3d48f1b94367c93af3092932c9d193a5af71fdf45108cad3dbc5698a4974d7f07f0a3fca6" },
                { "sco", "dfd8fc3e35720f38ace2b915f876fa4b5ff81fa12c0c67348ee9c724bd1b8e996b3eb29fd7e575c90a888c60fe54fcac2ed2db1157c22d0d532161b5bb370e03" },
                { "si", "5e19cc7cc3c7edad1694c858994bc27e286ceac2dd4d7d85003511c2a315ac6666e5d1b2e61d383d9228e59269ad3bfad6d9d509da7948f3835242817b4e4786" },
                { "sk", "5bac8eaacade6118c26c099447f2785136797fe802b784a6fc6f496336cc1efabeeb518aaf0836f0b542ce34509c8427cc75543a9cbca31fe6f87bb09928c211" },
                { "sl", "463118333d55ad1f498eba0329f80ae57c6b9375c85f474e4e5ed17b304060849393e81c457b4d672b2c5c1f0b294a86b6c38843451ee3c66508dd794634ca9b" },
                { "son", "7e503cc7ae417b956de84eabf8ec78ad96a75157613b11492e3cc77d26a4aa6d1c6192309d3ea1fda482450d59b3238c0c0eca6f4daeb6e187d0aac8a995a535" },
                { "sq", "58be12ebd51d3301c0a0d872202941037dbe7024ffdbd7b644a0c5a118cac993f151bd5d9782e481671c81db272687a1a647fe90be896a4bf05815b5e1c61a77" },
                { "sr", "8571c4879bdb7d545acfbb799cb446604c8e0c6c82da3ef2a565d38150577b4aae37d3bd14bb6abc0138b96874c694a55047f045a499789f5096d0632d6656fd" },
                { "sv-SE", "277b7bccfba5dc8258018caf65f1948ceb14986b641fb45b8dd7389dac9f0bd91225f9ebf434d627aebc7883695927dae2c65ccd3a0a9fbee84d0d0646cdf59d" },
                { "szl", "b4ce4d32032dfbceb83f0ed43ddadd31cc1a0abf20a2b49fae7819a535a1875e9a41dfb3199627813f96cfa9b8769370e1204fea4a9a05c0737ed4baed71f2f2" },
                { "ta", "9cf56e7e809454086c8a1c4151900615d84d807468f2f4ff788af5c494eec81637df0087e89c38b2e0b97ffb338e4ad9197994a2e638b64ad7e4fc2178a0ad23" },
                { "te", "a020810dc9a8510f7cce3ab7d7073d30c49994b662586b2703b6145d7ba3581ba4550cbab1a0166feef64a843bfde5bced930dff406fc70aae8063be7e46a18b" },
                { "th", "5299c11ced6e2e8370ee7af88b325e81bbc22cc490c177308b7fe275e9997a85e4133c89cb4ea8c5834f8fbe052874272edfc4bb64662276e5e758689ec9dc91" },
                { "tl", "907ecc151f31311440283862d83226503b195cf597d8c13c06898499da62eee3aa98e324db9b4946a8261bef97be99239a3a486242e1b202d6f2c683cafb6ed4" },
                { "tr", "c9f729b5470675a56da0822d6d27a434749acc1b229452a8c392fc637a2cc63219143219d3993165a75acf11eafb2e7776cacd2a9d252e54cd762456ceb5bd87" },
                { "trs", "20140f8f24baf44da5a34208b3a73f8a3746ffba0ffbb44a2074f20d41a442b020e7a6de4794aa50671fa16d3ee98f330fdebbf87b1a8210a3ce8848c870a67a" },
                { "uk", "880d82516a5944ed8af68223d8b5399a7dac7f57ef347415eb20329f4185e4e5c307e719ad51de11e99fbece18deb9af9ea2548bb36314bed1fddc3b0e5a4884" },
                { "ur", "6bd272e83f3af89645e10887b684bca2991c53e2c830b1fbb914692dc61f804d0addac094327ed85dc246c20955b6930c26adbd446125d0a2ef1ef9e19002748" },
                { "uz", "76aaa2e476e83beeedbd777ce28f46b288dfe1c1e6dcf4af2a98d12a369678b1768066e91e8b017aa6831868b505a6bf9cc416b1f5682633f42d01ab5b453b5e" },
                { "vi", "c39d7dc7d5d7967d7f5ffb01a301065a3c331c57d8e29e28920f8a1a03a90af38d742004da84919a7c98760201d23702230437c20fcb0ba8264ef6bcd8fb9f6e" },
                { "xh", "b535461efab83c2592e79f1a2a96fd2d6f655d13cc23152c078abeb88b4c1a11e877017dd24239ae0768a761910e4baa0817d1b53f53f2399d2c77222fe77f05" },
                { "zh-CN", "86cd5a62add605b3c99c6b96404f989ae5a8f1041a2e3c6b78623f868804a602d38928a05480e1227bf56265096e8835485a35997ea8368127d272e82bc02935" },
                { "zh-TW", "ffdfcfda1e35d52150e2036652869d44e775fd22b94c9164b762ff932593b7b5db693b74e87e76c94a20abffd56eaf3e5d5ccfe89a03f028aff973b359b2d841" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "106.0.2";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
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
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
            logger.Info("Searcing for newer version of Firefox...");
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
                // failure occurred
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
        /// language code for the Firefox ESR version
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
    } // class
} // namespace
