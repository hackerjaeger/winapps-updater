﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "125.0b7";

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
            // https://ftp.mozilla.org/pub/devedition/releases/125.0b7/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "0623a8808adb9dbc59a8eaa39763ee7d84e0bc7e1ea4780e30c668f637930fc8aac38a20a40da276f7c7486d119679fb5d1edaab5f9426ae5d29977505b52d12" },
                { "af", "d2011f994880d4bc2d737dc3e80f1f0faad70d38875465eff3012d6e9ce3b160759c66ea315bc904cffbbff98f4083a28d8cb4b52cb2cc90e262ab442f6018f2" },
                { "an", "bc76c7c51157a20549513667ac2f6e0d8abd8933981ac6821fb5704f2dfea8f8e189637bef5bdfb632b11d865445d3648dcd7119fc9d64c6b4b46b9957dee53a" },
                { "ar", "0109b9bb4054543064ae40a4db9620d51c98ae908af20e6754a9f36851f9548f63fd6411d666963358cc0bef8c0b30587a0619e781bb39519cb4124debbc9dd5" },
                { "ast", "b6f1383154516d3d791dbf9ab7d746fab33c52770cf1348efe7c94b906f632ae642fae574d6f0e660f194e4c9226a20f051a6b71fa8c6ba90104bf5a66bbf98a" },
                { "az", "c037591a6bf2484a8d438da3db11534e3c9066975f6a615e661ab392c5cb0718d4c9c94e52ef21b3ce8c0636c769d87f8f7f050eeeb7f1b1fe63a1d8921c5a5f" },
                { "be", "34d4ff2bb1f955f70f65119c6da6a7e51b83fc431b51ee1a590ac5bfb16c371a1eaf1aec167b73931353efb14f143d6eb86132431bfd2ccb5244ed3bdefa6c27" },
                { "bg", "810442fd4f6c5970c122cb6a9d3cf90a4bc7f2889b8d661cbb10d0d2aadcb9bb79e8fbb5e8421540f22fc8b45ad332f9f68536b7529439864c666d328523ed76" },
                { "bn", "0210d72a137ff344034132a48c8f77283d6d423e79808795468c86d25f0f98feff66ad58d6b38609518ebe347e5f017c1dbb4953e7a21664fafa545140a47d6b" },
                { "br", "f9814c375ab5301724502025e5a4ab6c8799eb152215505090e83af5029328e4ce626a4127a6a166ce23df25636263158be2fbd00fcebdfd2454515d54de5dd4" },
                { "bs", "5613be9b35e013933531983a67f4813ff806fd4de20f5d73c7b0adb6540f1743edc9ddc6fbf03055e5a8ed7fce57706338c9db6d893814b5a54c3e20464cc784" },
                { "ca", "89f2d2121df77b668bb36ed0d0ad2cdc117b329e3e6a7de569b229405b35dee7b1f434ca4d65e28e2348883bcab96e3c4dbf677f056c3c1d728a8b54ee34e11f" },
                { "cak", "328f42826b0eef97dc6d97e98761adc9f88f17d371c0b348351c9d2235d23c6e52942dc2e3bc8d495a9089585d2420d75573404f527939940d51d41f21db5b19" },
                { "cs", "60c0574bb58560ba8c470baf30e9468014e24fef43d0e027d8333e808261d57d96e4664e41f79afaf600237ac6b99a823bf7a383414dc1709fcdb483b003ba06" },
                { "cy", "cf41d7a484dba0155ef52b1e4b405fc9ffe2a2e976feb64d8c4176f575a75b510f92f04cf161a08ae3c8d7976dbd308a6b8874744920c112c1995aabec489e51" },
                { "da", "6c9aefb4cd15852bd36d7d6f061b71d45fee4e7d908039af2ac9a941050a2f368699da326cc24820b4491dedf0930b85b7fb4f272ad776575c065dc9dc66fdc1" },
                { "de", "0fbbe15931b79e597784edca618b5334f372d3b82de0c9e360c48f437afb89a8043dbc525ea96656ad685677a0d450aba2fc8c13409a2f90f251b4abd5bbe7af" },
                { "dsb", "b1624898326e3cbc3c0f8a35975c06b1b02db8f917061309d7e6f9823bae7a5d32d584c7145f58752c0372a380a8ded1dff9b88c8616e5a2776528cce1ea5bab" },
                { "el", "d3c31e939b3d1bd2437b475a89cbec9dde50a40baeaf81cdde010e970348177ca2e864b97b9a54172e062b35562e6c4f212d6a0b28d9492c2deb1c28ca430826" },
                { "en-CA", "1769ce336ab4a8619ae387939e9dad8e4d056e35c3cc45ce2dddb489859665f68338781ca4cb67188614584fc84c1a7da09b25469176445e961bd7b4761c7a78" },
                { "en-GB", "69cd5fa4d1df0397f421286c9576d184663d020b4b800f8a607404be7b7f5c4ccfe886f8576ace994743a2197049f938e5839dc93be17e2fd0bf9165f433ac6e" },
                { "en-US", "2e6c9b97e7640718598f6452e2912bbe5f8d3297b0f696c68bb48694ac405eb67d98b162e85113e8c14198b66a701fe2ab8ce73082b78e2278dc4ea66a25a261" },
                { "eo", "89219249cbe55e5d48f9f36b19567f5a0e00be4d06b998049d5ad70c47d4267f1326ab473fab1f6dfd9c46a739251d0a1aca041e13417c39fa7e3c3f75117a43" },
                { "es-AR", "15f8661e592fa7d8b3883f9b4c2fce6dbf90a3902b5923f7cae6e4ff549dc056ebd53c7bf73403bd573cfeb648d49d7bac1307e2f4f5aadb7e2a6c89e0f759da" },
                { "es-CL", "adc46ab70831cb22622d3ecd6ff5c9b6808862de00bf7d2032a422884d985eab58a16e9527b01d033cb6f9f9ee46112980b09fb5ae1a3fb6a1bee8178955312e" },
                { "es-ES", "75eb8e23e794779cbe7d992eca69b1415521e19467f47445deddf32fd8fbfe6814865a94e12b60584322e4c1f185bd3a2df5fe2991931260c26ca67f6541c819" },
                { "es-MX", "af53ce3126152e256dee7a76a90b8a4ea29b9efe2b26344a93c32951b18486d82e1bce3676c1fd004bbfa321091d2096bf1e21211e73ad2d14d9b437766253a0" },
                { "et", "0801b9d69e0260db63e878526abb860ff42829a0d3ca6e03b2b2db8ee166e1c2d526e31ffaf960a2c623c1109c0360778372b22292fe329003b21cd597d302db" },
                { "eu", "388e530b528529fa67cb7bdd1e5c80d893a733d86bcdbaed09b990044a827e53299f68ddee4288d666b9ebeb1b3fe782910899b70f6db1906cc8b49b5924533a" },
                { "fa", "cde35b4773afa0ba9e3911c146cdfec405309ce7145b1485d54e55dcf47deed6531add7e9d73b2a6282a56a1dd09e12a2c027fbd97781f18361643ba61a83daf" },
                { "ff", "7ccb496f44c905df1351a8d103f1af0c9ece5979ad5d6df6f165ffb9a735f50e22f410f396183a46c5628bebbbcc22254f02ca038362bde7b1bfebecd3d071b3" },
                { "fi", "cf901e0a7c05a97e3d0e4f37d3bb95c96239ce3dfcb7aa3bd22e30b025e74a701f6a7c46147dbc34b602e10535fd0dd36aaa6acb4ed086bb25351bb24181c451" },
                { "fr", "c8a985015e6b9b02c1482ae3134c02bc21b33ba50a268d700c1e27a7ef1ea6abcb0ce7501f155907d65c88fb6f4e0c0177350e8a611caed6fe6d1e1fe0a93cfd" },
                { "fur", "a7febb8dec143d48950861af11313e30b2fd1b794aaa99307a967ecdd918c118a10818903af146f08a915a57a5e35d7083efc00f0e82e8bd2ff72114e7ba1f7c" },
                { "fy-NL", "76753d888f900a75ea4e362d113e46b01ad51cef4a16107cbdc4efeaf7b7218c84d1ce6756019af238e6fcf44748094f6246a8eaec9c57c3ed601ea7e5d0342d" },
                { "ga-IE", "52c85a740303c130b36a1f4fe694855cbbcbe20c408963c837244dd5e2309f1aec9a194dafcc82c84f66592dd0b834044ab991aa2e838b0637e4cd7d84135606" },
                { "gd", "41fa81283114e0f029a4c399427b7707d47731875a49ca48016ee7a61a9d27575f98dfd0e4594baae1f641dc11de4807edcd6441fce79274114e2c05881d382d" },
                { "gl", "613a7ff706992f8f054c3dfde50fe3bf097ddcda811b97e01c13ba4cd2496ad57a4029507a8a26e32b397adbced594f7c89f9fa398103847370320b28361b4ce" },
                { "gn", "80152c07ae01bea8eb7bdbba3cc74212a6298b26602d227037e0011aa3342af23d95df7f00bb87414cc51306b85181dbcff74ef17336ac8395bf4586b84bcf91" },
                { "gu-IN", "9a18c65910da0ad94e23d452d59bc579cfc4915e3da80dd3e2b74c444792be91d06473406565aa54d5bd99943b0c5c6734f6cc6be62d1b5283fc7ec896cd5f9f" },
                { "he", "faee2a91dc2b8e9c13074ed8ab0a5cc5400d20bf777a1af1d7e6fa703ce84c3231e505d57dc3796d59c33a195568de9a0f006454a42a4a1d672ca2c64631d5d3" },
                { "hi-IN", "e85d6d6b1767878c120be91b23ab2a43540c0fc30b89a8e807e2ca1eb704bddc2324b7be476cf09be5c67687582413f6015833bb427bf666e70c6f583c0278d6" },
                { "hr", "5e6aa0402a12c9f65066555d261bd67b0961661f645b8104b60ec83b56ad32d6e741778e26ef309d7c9aadcbd2a9df3c90c45e742193fb2adf9cd35ab6689d59" },
                { "hsb", "0ce2086cf936f846f5a976689cad909b3a7216de60e44540b4d2a142156b60a3f8a3c3c2159e3774a268e9f596b600852699f67b3b643dfc7a848ddd7da0c72e" },
                { "hu", "9f625ef821aec18597b077f4779561ef8b69c237e4182778ec302753925ee306a8a5ae98a79667a827327775a0ec9f8dd35cb34437cd3d17931060da097ea2df" },
                { "hy-AM", "95157dd5eaeb3566e7e3f0ad902c9112a8a6189c6b975d604276e0d1e427b196b9ce770e5c2d5839a466f2e23371bd4279cf062aca422cb6736ef0cbe736c79c" },
                { "ia", "b170fc63b3e17d26607b7da821df88e6584b990e9159813d6ca37b35f9cc45203ddcbd349fbd0c72e167d3a3d93e67814dec03260a3da060310f0a6ae7521486" },
                { "id", "e271d8126661e651f3725e06dcd4e0f3fc1959e26391be3f16eba71eb223bc816d8493f2f123758b95df7b86ea9d3ccb69a591a1531a65a7f3fd56658d7b534b" },
                { "is", "acd7276055ce93718e0e0e58357450f62af975f2bd833a33515297be4c0fe3566ba0f564f7848a7775dbf6c207d8dfc852e911c8b4830bad93482fd54531037a" },
                { "it", "78957b77b30a08d777705d328f41061ea38e998cddb1d42f4abb6b10583099596b2c055dfb93ca9686b980ba96abee1c49637e17974bbf9209c210460bb2ab47" },
                { "ja", "64ce8fb6210e4642e7b6a9539dd72d43bcb4061e1a9d4ea10623871684f95194f2c9cd736da641d2bb01fa02641d82ef5b65a09ea70fcfc5f413baee523a71ac" },
                { "ka", "7492d607e34d734637d8ff31f47d0f489107ba424b8b610380642a74ab0f96a549d85bbbd9be1a534c3ff2fb6fa4874ce90ebedde41fa2b30121d5b26699682a" },
                { "kab", "fbfe8dde7bbd21ca21e90d58a1dba717fb34f577ac0c6bbefa072d9416d3cc4d86bb9cb93646518e0d3688ad01e13e59895cc5860ef071e31049dd7152e80625" },
                { "kk", "286bcfaa54b852d793627918e2569414031671e6d3a57aa1eea8d74dbb1915fc1264c36398984ec9b859eb7155bd734c3d49d94d25d5e9ff90d8ded42d023eb2" },
                { "km", "b4f9fd9fe323c187b69112240041ace3e1a8879b43e4df1fb728cdcd3bdbe737408dd893adeecca3f8452c97e8346e9d96fc47efef777c8204300fce2106e472" },
                { "kn", "0f8ec8ea95e4e4860f32d12f97779af5e4f8d31404b814e9ee0ee61217429f660c81c682a795c38bda5c4a9b9ac8dc8bacda6bc9b5fd45dde81d76269bc7c1e2" },
                { "ko", "5593b6119e4a77a12a747ac255c65c8bfeb030e71e2ef5f9c578527decbcba37e19fe3469aa0aa4b895d46dd00424cd132c1f5f4f85eac736c1560a3f0a806ab" },
                { "lij", "15c892fe19089839d3c702580e8e4688a60ee695008933cac311dee8d5d679c2aacdbb8f7294b10ca769e891d3f1814e1663ed9f21c4234791f68151f05f2766" },
                { "lt", "2286a654fffe52ec7d5cf0e987a53e8aabdbafd8fddb7b1cbe7c9fcdf2a46b7bf88eedfd2f34ec6419e7fdaceac84faefff1634ab845f6b23fbc9c01d2b52f36" },
                { "lv", "72eb35d554e8093510d72990862105cdc8e59eee2253a809d6f53bea7a9aa9cc75aa6bc0d6216930c80f6f8cc892c023858a2e512eb52dfa241593730d5b4cec" },
                { "mk", "1599df6a073bc2cfb52904a8953839130fdaf515a136822e042977f7170db978b592e81cf8ddd7a3787d6b487a9e2a1ced6e5eca1b78f591ee0a2dc0375e080d" },
                { "mr", "e6db451efdd5ffcc2c0b61c7281e616a62a6b204cb34c418b2ea8590d269d3ae6004109a64b689a1d4f59d7908a339e05f6c63a56e77216daa9f84928881f784" },
                { "ms", "c92ea1eb4a79f01a3d6d662860ace7a0c03908b7b3896094269abca52ef46bab0cb3f5e479dcb65cb3bd0ee34fd68854c6d8f66e87a810e3d1e7a4abd7b31358" },
                { "my", "680a1b7ee0f79f1b1693c3f89413219df026666033560d262d84de95b0986ab0b561f542b600b16d32878816e05853b02f74577b9dc71fd8b8a1733ce8ca2730" },
                { "nb-NO", "1851ae3ac1a3fcf9b28574dde7f3bb8c0e29413afe1bbce091727d9af24ffc6b2e2862e808c8ff8fbfdd52f86384adb5a0b74c0a26ada3aea8bc731751073dae" },
                { "ne-NP", "091a133b6de6a43bdeaf177f88c8cd9795fd63f3867f86f4345dc61f827e16ac4f014c945f8d265c96f558402ec9905f73641ab0abac638c53ed97f3153e4908" },
                { "nl", "2edc51aca08d803a9e1ba79448e2eea6e29e44fbccb65887fe9dfab5eccaa6d22784246872526459935b2cffd694e88a3e1c1a179adf842ba11b601f6aff6c61" },
                { "nn-NO", "b89867aa5c5a3e6843c1f8c50e7f2a3e0af3e412a7f25ba52d65688fc399cd00166ab29308c61929ca8751c8f41450727141ff2fc0d53c4a10e7fb577361f671" },
                { "oc", "6724150e3dc0ad36f039b946c90045ad81ea8e0f4c5fc08917cd120075a9a58fac2403b82a9d918c46f7369c787388f4f5a5e02ec19b2305d8d1ea2b4f2a9743" },
                { "pa-IN", "52ffaba58d06a0117074d0e75902452f8212e76015386e0cfdf2bb90105c5a9f8aa80f1f6c6fac3bec281ce764bf4a1c0905ba5ea885406d603889a04a6b4a1a" },
                { "pl", "b4394c80d9938a681293d1a172e2f881950a21cb7cfea4bc1fb714ed35b2270dff62a57fec2e87edf6eac7ab7e577e23c709b1b096ceb2f974b49673a001fd71" },
                { "pt-BR", "c60c6b2cd4aed381b33dc017fbbe03c3c2d39870cbf327af3ba81fe973a18e11f3607b54a5682a06c9071c4a2d2f303a7c10e259d7a5604480d93b11a03e8d09" },
                { "pt-PT", "cff1c44f1d4838c1cc24cf5f19f41b406d25bcceea9201ebc00aee7c1058b444ed6e7523565580f765d9545d6d7359e1d56967f6e99add1c5af83be8de6365d7" },
                { "rm", "df30f86d0e52e9e5fb180fe1145d1d1172851e3055edb9b094899a18aee6d3e51459de368447d3e3bdc74a511e0ee5836b3e21a6827877a6cc8bc79e78312651" },
                { "ro", "3984e406ceeaf8185cd588f297a0d5726a07b5b2cb364f1cdd3597c0de4647ce8e4536e70bab12b38a3e8d9780e568c299c2511b1da71665a0a92f1187d4e2da" },
                { "ru", "19a91edae55cfe3ffd7b476c54ac2ea772a9980093323b22858bf057e6a63a95cfaf68506af540678e59ca25f5145ac437856ee36e68e2ab783e472d25dba6ce" },
                { "sat", "fe9c683999fce57bb2cab2604c17c4b9863706f15ffa80c730bffe42d8b3b5812850fc03ae2a2fe4ae77ab03c062197a3f955d4f8e38023e7709520c0999ab09" },
                { "sc", "8ef44faf28e0188aefddda11ba25c9c8ddbe592bdb597ef86766fee012104cd4d6884e1f2061e56eb188513e0a9f991476a1f630c1f747d6dfdca5adacb5fdae" },
                { "sco", "95e0c2297785c71bc81f7f514e72bffaf6daa4b1a4834cc9ce5acccc18ecf2ce0c92e210814f65524f893ea6066286d068646c53c68a188535c39b87e07755a1" },
                { "si", "006970fd990525136fe4d5d361be5ff50d0d0ee09e65a4e8832cc6d1fd6d1351915d7be6312b02bb238dd4987dbcc74de6c047d2394392111a369fb704a0e627" },
                { "sk", "0cb125fa11079fb0846e4549755af1f6b68c033f4d2eb611471da406cea5df82f91eead6178827297a5d941a5c25fc34f4ae983d69ba0c6a629091995a7679b5" },
                { "sl", "111599254a7d667c0b49b13c6b9b202183074752d8f59b87f2d8db45a30be9929128ff355b9d58f30afa60166a15693b976c1ac61833d84dd9ef0a5005b21df0" },
                { "son", "c6194c97d122bf25aa43829bce9e77cb1492df3772bb5e1ea5fd906bfc8c8d1b2b842db20a3a411db34c68630896af4368e5c42591de2031428c2d63b47d448c" },
                { "sq", "9c95ee16fe71d365382b1d63926d677aab01102834e5e4c3eadf437045397760e236a049eeb7614d00a7aa2c6effdf211de2d8dd22b8ff3ebca6cd7556b1f7a5" },
                { "sr", "0c1575cf29d49cf13168cb9b25164c5de12db10f56727cee4671163f214f21049313b23320154c29c9fdce53197fab21718d6168038a9b3242b895004547e2f3" },
                { "sv-SE", "e10c79b67b9032ece255531c24cf3f28baa77e92137561541da2f8cfd5ced49aa4087dd08ccbf8aa53cf9e4f6c62c7a6ffe769594d9ecf21fe3c499d8cfb37d1" },
                { "szl", "fec757c07c7a4a720bdbb8e62545f91fd63d2f48aab38578dfc9cb332c4ad4df154a59fe864c94528c29b8f1a1f5319d2e4c0fc812f1135e7d191b7e2bd62cc7" },
                { "ta", "680dded618249cdcbb85c1b91a7817364fcde09df2e83ea4371e02ac4c92131225fb12b82ad062d80616b6d4e76292834b561b580b162629bc1a8c7f2707e8f6" },
                { "te", "012bc058974b6ee8e0116c7d599af9b65fd9764f3990b5a6f04c14a908af23edf4886c51f4a028e47dcfd6708ec85459c772d1f241207aa903f99415ad00825c" },
                { "tg", "31b61362590a9fa658f1155b2548b0b304ff3e3bad119d2765b5d8794387ae852361ba7d7a437f6349fd400f921b245fd3cdf12214774c4438a8fec3d179ab75" },
                { "th", "a9b14fc76b9121ba5a738646165de68134a29b82f67594025ef5883cd10ac490551f962fef0c49a76ecda4f0af57dc95a526e5ca44a16f7a74f159c72a65a723" },
                { "tl", "b3ca77a55618ef50dcc1eb93efaa938156c80e3893e62093fa0151bd2d7fe88e3d22907b16131cef6cb4e8575a9cc0399f52a3909e839415c5495d12b2bd3cd4" },
                { "tr", "3fb42c7b962bfa813eb99009ee5491a3d0ca41d65022bd7c80111767edfc0669010e5007635027e4e3be49ceb0aa36d7138b60985a9f68e7e6cdf2f8569e49af" },
                { "trs", "9077fec1cb04ab79fb59d9161f1aa948aee8f43f85501ad9a757b4db76d96fb72dfe9f7b99e4817e1e185590946b82d1faf142f66cf50d6d79a14511ac11c6c3" },
                { "uk", "124b148f34bd6546307063372a36d7746bc6a108d6906b15ed28ae2a30fa68045bdac0aab9d8d80fff92570bbe6864052207ac3af30d03d5e52d53f2406fe3fe" },
                { "ur", "095a75538130dfac9cad7928775ab2dc73c1fef718d0bf1ff6c79ae1ffe40bab145838d0a231d894a0de9019f0e2e5ee5b4d7fe32ac3f434dd7d2f5d78f9cffd" },
                { "uz", "0d3eb091bdcdfb0fcc243ed0bf8ebaec705f91765b7367db912a320c024c0d0162ce1262ccc04e0c1426bd27ba94c6d6f462d8cce64f091d84be17812c371ae1" },
                { "vi", "e7db819c8ef295939eb7f953d91f0b8b89f4a7aac88e77964488d5fff1ec7bebbd0ef7e88b15f26d79a2e66dbe7a8a8328934d1ce96f5e69428ed82fdb79cc2e" },
                { "xh", "3384bd66bc65a58893e6ab2b43dfee8bc1d50315218b9cd8bf176bee3a6ca8491091787fe3d21dfae34017d0e3638ae3b8d8b28767437f111742d6a5a5cbd484" },
                { "zh-CN", "b08c1128afaf2179793522420f5e9e62f47e072f08627be2af73685248a5285278746631bb5af9f52dd55415e992383d06c7f8c966e098cf054dcd1000fbd216" },
                { "zh-TW", "6364e8a2a4b3c4e81f716a11652bd788dcbd2dd3073fff5158f16c789f3565415420ea2b912171a1d640f79d90c885b132bb88cb07999b7a5a16d13f7d8fefdf" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/125.0b7/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "575e66acb20e1c9debaa061c5d452dfcb028803870bb199a495a68c2ea758963d3fcd7ec7183c1fc8428dc505564b949f764416c20b75cab84e2029b31e49104" },
                { "af", "747e8c30176d049bccad2e8e9f46d5bbd06faa7ded221e1c74de7a2c62284b99d4af01b5d3ccc43e2ec3cddf291810dc475c5c8a247817a5245dc01e3cb6cb74" },
                { "an", "799e37166908d090aad1293326a8fb85d83228843a9bb1ccc4f60a59abd00f817262ca13bcea65f3270361867ba3ece9cf67272ea1400e3adcf68bb23f5a79a2" },
                { "ar", "4793df5a9dab1b05d4450b7a2810139f00e430695d525fd23eab8587914b8d1e5034be2f82244f58890262c91d4aaacc5663de0bb7a1be6b2f821a7c8e5c6d30" },
                { "ast", "971dd748873e60cc253d8e60d572f73aa48f6068bc45c13c93df26853a0e00ec1e6ab5688c6b7f08791ef0bcf8e1db2409c9f6eb217fa1167745fee38d1f416b" },
                { "az", "563c9407f033e5c585cf0624670c7459992b752cc09cc49897ccca0e9fbd9a1614cb9d11f4ca3ccdc73e10ef96c2fc41c98d254b39d3ccdcca80cf1853f94575" },
                { "be", "e23b7526e03207077cf57d5bfcb2ae2dc36830a9be1d3c67804bb20cb3b851cde86d13d555f5264c7f6d24339a137011c44c9f58541d0dd11e16141a00296b77" },
                { "bg", "2d405d82770b3121d3b23e8fd0d5aa2ce81b328a3114309b8369150449b9d64e86c7dfaead3822429b259af7ef6e74136b2e494963d29f570abe18d51c320c54" },
                { "bn", "85f28400e9414826de3381c5c0e0fd46a3fce08550a52a894d20c3fac63bf138d31ba45d4c011844b8caa75d211617739e0ff310022cb3cee64d3aa2bdf3c432" },
                { "br", "108c9b2f064db87ef98a3958ac8451b7227bba472ae7af32450b8ce0b8f3360732f60141601c0cc5fe3ea2462dc79ddbe9390793ee75db2cb45be2056b83cbd5" },
                { "bs", "9eace162a695da35035feae86949b2b7952d3979a36ab1be68732cb365e5537b1d0cb6a07e4fab4bb6eb9e811c04068723a5aa672e2c8e2ce8f687c73475c39c" },
                { "ca", "b5bcee8e7cfad2e3c0fde25ccb29a0ac7231a4893d01a64e8fe5e2cb2971a475e68a2f5d738adf419f94ac873b862c16987f1fe6837f19838d61789f619e8600" },
                { "cak", "f4607a16048c3531658cd11fc39aac83e5522e51065dc231d7cb6dff478001fbc7ae78fc3663cbad2d755c6a89d16fc5067a1e1c2ccd7d833200355448b139ac" },
                { "cs", "5afad086d8ca1f966d8b123366c7bea4988ad84dcf0dcc21d35eaf4f5c265c1c1e344292b392dea0c5a7a7b2e7b5389ad2fa93cbb62347ac71c9d362659baead" },
                { "cy", "1dcd3970bec3ea941ab7d3560d00720a139f204714f37e27516842023f680ca0dc8ef8d8c3a7a6b3142d5f64b0b67a730b942039f8cc3426b27c9b5a6f5e2881" },
                { "da", "f28b03d4b9e2746d5b6766b50bbefcce1cb154a342f98b37e06fa4b0e151a3b2bc7a0a57f4f943e5db518ee31e4f593aaf4c5e7539e8c846be3f1a1cafe7e0a3" },
                { "de", "bb36edc57c8ac6501b9e79e3c6786f613f58e9f2b56401d2b0dd4aad5cc728476ddf20cc7e8067117ab9a4afcd967aa80b95c79a6ea445a9cd7bf0b21067e47a" },
                { "dsb", "274265c724ef72c29f31c9e4bdc9c1acfa88f1e9ecfdd6a79ae6d29426d7a82de0f882955bb13933b9cec9ef388b01dc09e8f587b5c6d991c283fb6a0e040e43" },
                { "el", "118f7a76cce31dc08d636f25273e93aefb8ece096c9141b5e6b2dc5d8a75bb3a630e67182a038003e54051592fdc5a9c6ba20e83af2733530c7ad6aee75a2975" },
                { "en-CA", "fadc06508bb63286a2314175759e8cc79187941664393c5b775428bef06fa96a3946ce76fe630f9ecc1ed676c06bfd2d5da8b93c35e8b8d3413cd7672f3ae28e" },
                { "en-GB", "56a80237bdef17edc4d612634d0b377438acfd24fa880452a542d53076032bec13371de86c21b64a007a553056fae3906be5425946476e6309310ea760b09e19" },
                { "en-US", "0fd7fd91406819db3d31b1bd2e4dc7aa98499f0ceebe85daa7758cd649862f73d59e20722ecf1b99c738e254fe439f36b9e3570a19c7794bc56f4df2efb11ba7" },
                { "eo", "c64ef9e03cc28cf21e9724af017357ccdeccc8f194c07dff08dcfd8f7367e35fb7b048b8907a58622e49df21594e325860d3020c7754f9d7eeab9d03a5b73278" },
                { "es-AR", "4a396c39c455448f4411b9998269916c2f6a746f2e80a53fca94498005e8f496e99f162414ec552d1631df614fcdc2969bfabd00393c3104220e888c0b331812" },
                { "es-CL", "77acaf6def09f4cb66c4032f754cc067c5ca71b776d71cb7de4e0b4a0278d8ff6e24ce24c9d9defbb12028c7298f1564a2bdefd9e5c08b57050d24952f95e86a" },
                { "es-ES", "fb0e7677dde92a7fadd1e335d067221e215d314cb2a953624b13af4bc123f44b74c0aa0d518976acde303b9c5aa5aa3997003e074e168321e79300f51b4359e0" },
                { "es-MX", "fa74dc8e6e66b9dfafdbf59081bd8b87863494d61d9a9d060e4e06f2eedb59671ea29c0525a37d5b36d8b379d9ea1fa95068654610c9a4f87a7231ad08505125" },
                { "et", "024cfbe2aed33bfdb66e5d6fc3bae9b954eb0cbe52fddf36b9d4dbf9a356c56482cd6f444d10f069a5592e0de65ec6ea38d8f767acd23b74eb96bdd881b6c2f8" },
                { "eu", "2eb53c68a8877e8c95640a2eb0884120e858ef86b9e8b05b5d746c5c70b14445e614b522edf795bf45f9bf5882faa173ac575c83d2b540ccb5760714fe1d91bb" },
                { "fa", "ef0506926a9a9fde75a6c424a276534e2fa92c92b06cede1ea019adcc5bf1f0399d3eaccb25a4c250b9bd219edada95fa3fe4d3b3e4e96b97a1f96d0bc80e934" },
                { "ff", "0f74b9d9a24e680bfeafc349b580b7bdbae27d8223bf5edbeadb60f23ddf1fedd5968842f19a0949987554899b1846df2df1ad77e351600aa0cd63d64e2ca495" },
                { "fi", "8e50f8f73ce13d0095b3db855b3084b02c10edc3d25dfb2d5f9504b778361e00ced1d8344924e10cf6a23a6e48e7ccf9eedbbc4ee66a8f86335cd8a09a321112" },
                { "fr", "7c39afa6fd4e6a4289fbc726b59fae2bef1f38ff5cd378b3048616bcff24c658c535e5b6c3ed41ea084bd2a9002b1996726c0abb2b58eea9c67d4d8517acb232" },
                { "fur", "c0c6642d49de123cdab40d004c2a29a953b05b4a7becff450a064fb1a6201584fa8226fcc0eb98d4a8d24d527871ed26cb61ef36f0330e1eb658b5a8aec4eb20" },
                { "fy-NL", "c90983c5d561e82f8e6d3f88b7be7921c6d8a10828372f1573e7f9b7af81902821a6b13fd610db3577be406dfd97849a01d5a4109202bed0a91d9b1a93bd80fa" },
                { "ga-IE", "24ff61a8f7903ba7a032ed724f3ed26a199a4bb8235f58ba927e46b6695a41e93c472612ba09b2372ec926062c3e8ebf69181ae9b8d762d2832abb3b8a9c0159" },
                { "gd", "3ea76b2a9f9c1f68c0276bb969ff0cbaa15d5dc174bc722ed0fc26e5663361dc8b9df2876ecd883b4dad30f13341f41926c8241d7c1070ef7c86aa3d53e1e85c" },
                { "gl", "694ca014dbcfdcd7a1b40c3bd1c087ff25b325a4e16c1747c24d88f9932bc72a420a51ee0d472055507ab571c99cc2a0a5079811433f29133fd7684047b9c405" },
                { "gn", "bae84b2771e317fef87d8620556f126394519fd0ab3888deb17123b7c23749c75803e41d04736c091f98588da1ffab72c3abf6418c9a0e8af4a1adda3eb31c56" },
                { "gu-IN", "16396e4c682604d7f7f4e3af7a4c85e3534ff028f77062d6c580634cab7aa379e1f8145807b09f52624327ed8b44c76a8252a5762a9c8194eda5cc1e965824c8" },
                { "he", "a642b4ef152bc06e1fef704d115a798009515910ae810516d8a117111beb51a56518b9ca78593cb31a504b9b9a9e7a5b57a3023312bb980eb86dfe771fc0fa14" },
                { "hi-IN", "87675a50e4f21c781c63a6c5048bad217650f4d8e60573ce55353760ff4ab109aeec3d03dda9dc94555c39a7e2ed69ee52c17ed4d3ac79414d6cf9f9a0ad8dc0" },
                { "hr", "874590b98485c7b694a5a573cd1589cd92499a5578618b0e1aad27cebd9f065344b7dbbc30d3a33e27a44358767a9ce7d47efe65ec04963eb1ee1b3ea0ba9f0e" },
                { "hsb", "58a6569982e635333a4154b7994780fa3539599ba3c53a207ed4e13b7ba769fba485fb4984274c213f1e3f05694f347b79603c7c885a84481b4c4a0e1c3c79fb" },
                { "hu", "69b53f6f495de7b7b2f67ad169874cce8bb46ca397f6b0a745d76bcbe2bde98a977a3b209e614b74e212acde37c03b5716c2cb200752919f6399f0630c20b9e4" },
                { "hy-AM", "1cd641ea78b6be594d99bb4d223623616db2447359f29902a75b3d343d1ab7da9b4cc4c8bc64634f441196718b05f3e2758bf8b95c560a8e9f609238c9cbafc5" },
                { "ia", "922ee1eafd77d404241a1bfd79ad457eb1b89544c7e2618ed4448027a6b463f3c49a23c3157173ed6c543e89617d28c492fa8e8e247a145ed4169e81e3cfe8c6" },
                { "id", "ca0e9d2ba97c2182b11df8a0ab91b263da1a544ab33eeb72a9132f2090aafa5a6dda42d75fe308096fa9b294302712bbf38a97fa501091307d0428497e4e4de0" },
                { "is", "54d9fa35b8c7fb2446b57cd9dae6503855a0025f4fdb3520858a9513d68defa5d5df0d5ccdfff2225f05d8ac9f59fc6eb88eed9a1d4d00ecc0bc085679874d80" },
                { "it", "93e19db1c2eb2b9475514327f4131feed07fb551bd299314e12de2ee72a311557d8e709cf2bf0e47d5af7e1fd30d456db72f694f620943056308c73a93cb3cd1" },
                { "ja", "863a858c95f5f5bbebeca9fa18c745921b3d51f74b6a487d026089626fa103a2ffe892647fd4d4ab5ac6398fc9abf11ddfe1a8c976ba757acc4046847b40057e" },
                { "ka", "2e5212ec60db192e91bb4bb36ac5b3ff46cbb39b4b98d1f5aaeb3cf5ae6eaf36cbbdf1cb3fbaebc7bc93b6becaa7d7d27640b5eb912af6d95be0ec07551e3591" },
                { "kab", "3350f17381eb6d009fca4500e5b036cef1c9854211cb505ae59f28166c9f8e9e35a30f5537fcb43254b1b13b65866e96702f68b62215e4fca952ca727417339e" },
                { "kk", "2bf8a3d3a81044c8948c3654342983a27cb95255cab54120a9e55e58899f167a4c72206e61f8701caa9e76ff9f834a3542b40e2b78d9ed7753ce3f1092bf4be6" },
                { "km", "ba6db2962f36c53c961cf403f1e1de1f114184672187a34abdae8466f78ff5c8255fedfc3da0248590b0c0d1de27ca9729c0f6a7ef718b31b3bcc6b21645d665" },
                { "kn", "dbfec17539b3129681e80095e261ad8b758d89e3df83a39d1de14c6528359548474e24eda61805d314adb30c863e918d147b537b06e8a954b3396e0b77fa4062" },
                { "ko", "a0699e5a70ab43b4a949dbdbfa78c4f7e497b7b611ede16cbe498745f3d5865501b3d2ebcd16bd03a0468d7d674fe7d46571b906371fa0ca16a095b23a78dc76" },
                { "lij", "bdd9c664239b6cc7d2e2151bae67f7485813c24c1ecf70bf790f8b0e2b78e80f22f556555a78823726dd81d2993abaf0c680fe6b96e4c3b7fb07775e277f8f8c" },
                { "lt", "6ad6136a2d829cd8784ebfb15a55ea527f60fc248483fcbdb08cf32be09c24bf5d76d23d3d3e583fecaf8b5b0062639849b6bf72607cec5677e3ac96d911f63f" },
                { "lv", "3589e5e190693602b25b68328a466bfd92e6e06c9472b2024db70a82e1178578d13eebad8b62eb8e6d0a351f5d84f23b9771f52206e2ddfd0b5ae14e8358ddb3" },
                { "mk", "1afdc9d9b8ab69f33a44466f206a8348bbc2b15991aef57c5deab16847e8b8b48af005a9842cd28f446f4779347e5841ffc5e02d7d994cfbd7387fe0c36aa067" },
                { "mr", "40bac04d109184105285bcefb6a508cff7f1e78d47c51f28c5a5db20c7454d7520c51a106cb956f195a1300f87fdc6f069434492fec92d48aaac796bffbf8c7c" },
                { "ms", "51be08d01d0db8f9d207732a89843320a236a6943a6e32e58093fe3d79db697de9a3aaa366becf32d2bc645a04437473b1143aa08af3aaf334906a4e06f49f24" },
                { "my", "649a9311f11700b0e357ca21cb7808b4e6c66531dc2ffbc69324160702143c47d1444bca2619273e741ab83f1b4abe00ec6d26b9965b941500574988a28e8c00" },
                { "nb-NO", "8e38a4c2564cfd7ae94bd285526f27ecad2d47c5ac485760c33bbe4658f1de36b0616ab456e61ecc242da274a6a7a087f17fad342ea437a4ae2797f20abd22a5" },
                { "ne-NP", "ce51689174938425642edd8424daccc64aa454beff179c3d454841436162fac4807c1736f9d5ae4e598069cd5f427f52929b670cfeaddb00d6bb5cc9ab486e77" },
                { "nl", "c12660aebb63475d3399417043d05aad1e69f10af8fe6a2fe7583bb6d46b94fec70f90ff5f26606d5605211dc93f54edae34414cbd17c2073aaf740627718f34" },
                { "nn-NO", "c8de526a4b28e5853e3dc0a23df49db5e6c713310b3cb7d3257c46cd8102ecfea8b442bf26604b5994e5c9bb2fa9d276dd307a6660b459501695e8807e6fb877" },
                { "oc", "6a7ecc6e41066c667f1cb0c52609210454d27307e3cb683914747938f002c098a165fe78e307b6f0bfac73d33b6bd4d2a5a85199376f07d4cf8ec663433139a2" },
                { "pa-IN", "5c1df2d07f4558a2f2ad1166d22b4903a44e60da5e32db5a0daa17cacd9b8fe5cecfc6aec886fd363fc82e65851c39d71227bfedf37b1041d359a255e1addb80" },
                { "pl", "19b636ad3597887e9248b9a295c0c2e383cc1a6c5b59dc3e72384af45cc881a9894a209f5a5a661bc7dc442192f442af2c95feb3bcbff7fed0e00e83f5c41ba1" },
                { "pt-BR", "6111fbe7a54e63e8e97f343e51baf548b8fc46bd5fc8da10338eddd95777c869f8107c635ac2076462aaa0875e811b329e40d2ce2132b493a6ecc0c8b72ecb82" },
                { "pt-PT", "603d4f3959e008029113903c728e369aac3ba6e301cc2118f7005ed51c6b36ff9976d2016bd5872b49a3c25c3af7c17205dab575e71e591db7d40638858d6246" },
                { "rm", "ed56a2875738d030d89de712f22deca30daef418769df806e77e485de15d5acf14b553f1b40d397ff4771e6f7e5700262d0568e25a7da208dafbe2adb7fb236a" },
                { "ro", "bdc67f6cb11178e4b17285624aaea0fb426587a9a4165a9933715f08d6b86255acde66747760b5dc97b48c0dce0551812aa8d89103bd1dced44ab007f5869225" },
                { "ru", "f2f8964f644d62038494068c37198a39390d92c67dca201c5a005af189aea370add4abe9cdb85f25c9361837caaac92d41045c6fe1e92bb1d3f564ce6093da41" },
                { "sat", "6b4db4b74836cd642bb7d8814570c2902e034b2dbee2843b12404ddfa7963e4c2ad26804c1008581a0e6d29681061d7a134a722278a9f2a6cbd85f66dbffae3e" },
                { "sc", "eb0e6e627b0ba5b824b8217e248639d59de17a92e0662f8e9652cfe24374551ae6e7e3f0fe04ede29a1171e039ab172d0c5a3ec41d5cd628d57aa20ca3488c29" },
                { "sco", "4480aed00550bf651e5d4f5c4f2cfb7cac7c2d285237a0a555152e683f9cf78a483e0b310320e12a7effcf6b586760134b3525336c97b89e9c916b3c026ef850" },
                { "si", "cc25d2590c4d74051d3c8e943e94602fcb7d71c893a9bd2c32329a232021149a668610fc35ed81e50ae687a38b80bf38328f937a4b5484883bab687e207c8bd8" },
                { "sk", "e79c0eb5c1aac2854c189b83d98eefe5243eabf998bec348679ba054ecbef61e3dc4d60f85daedeac689dd1f14185e36175f0326427501a8e8503dffbd788b0f" },
                { "sl", "88e3f51d2736391e2f64f0f1369fb5350bd4f52114b59f433c061aaf1908f9a4c7461ddff3d12c0f22082c54a2a6646db7b4b2fdce1c8067115ddf65daea7d0d" },
                { "son", "7008ccc26c7ba8f449b573874941e4631282caf7760531fcbe4b64791a53d5b502c8fa3378ca2416f968757f9ceb6f7d1b9e952e44d8bc2ac5c29a6dd657762f" },
                { "sq", "0fcbf007ce0391bba630292e48d01121fdf0a615ab77c445cc25e9f3284b1408e06ceee1f6bc5c4118da168449b0026e8c6031d62c2bb5ef80ea8cd1f99e2296" },
                { "sr", "53777db501ed2d389c73809d7741ea9c85570a503317af6e3ba1ec0aea7fe3cfd075058d596dda0d647bd31c465f0fff7356b4c8fca294fb49997b5a03e8dc6a" },
                { "sv-SE", "9ecab529a14a79e174619db1effa319d3b3791c98515483028d302b907b08eee90f42f1826c2f9d7c8cad40866fa5526618a7f74486518834453f0435e3cb624" },
                { "szl", "4e4d88a003a1738996d322449e22740b9f612bc0837a0e24e487d9514cda9b8d33f77c0f2a7d70dc259a3056ce2b0b12a0c525013900bd61149bde939a327be0" },
                { "ta", "61410afab56252cc9aa1f3df2f25deb0ec97f0e7fc2b5cb2e796063e701e078a9f21ee8f4c5c52352ed6419e138777112a0f1fce64969f8432d398b4d6171f08" },
                { "te", "ac8dac088ea575d523b05ce6ee4345d6c964276ca1c51cfae6eb8a24eb92ee33071a33fb525a72e3973a759fb2077b4363c2524d721abca2159d39463b3a2b14" },
                { "tg", "b848bf504b629b03b1b20eb9c19834d3736a4435ca6a596f0f879d019a6d962253aeb64b110edcadec54aa6497a13a902045bce2112bf52433b11f3e0a8c7ce3" },
                { "th", "31a578c40448ab636eb64e2ad3e1390cf0822b1f37d3051137f96b90a4a800046e79b524abac0ae3fc8a1397eddcb356d884bf2773f9cb5914df04315441130f" },
                { "tl", "4299abce433e8b4306b4d0921f28f354d6286297806a918ca1b347a2e69b7b6df2933029c331d44e93809b72cc5167f906b5a44b2d98a10fda5c6e44eeb82ec0" },
                { "tr", "df613d5b6b5863f6a3f22c35210e565dbdc5e781a577fe1555db9e3e5054983df4ed863cb8509c196daa0296b97fcf072e53f6429321b4e2aaa5bf0205415694" },
                { "trs", "6407b4e4a98c2bbc3a092de00d3e6d37303604ea05eaf8069d253d2b0815d55211b8a99fa3ca633b2a14bda55fe6b0e6e5e351c814bab8e56d200a3789e2ce74" },
                { "uk", "caea779df0367cf2e6d627fc3089a0f2272f00c187f6ef5c25d551863c2ab2c9b4b2f9575765d9258c821925c7ec16e00a7971b042b4cf6ead2e34214c27ad87" },
                { "ur", "4f808d469b07d31181a18587bd85969831c2ffb88692ec38f56f6411849e81297868c0ec454d2e94eb74b42c3d51251599eed28b6aab127270538dfc9764286f" },
                { "uz", "43375b08fa92008cc93a3e95ef74f8f4531690bad289c959e873a7a78504e451b70789b7ccf5790f0d3e93c2dc08f5da2ea1bbbb5a753c9ef2f9323abfc2a87b" },
                { "vi", "7ef98786d234e1147e59f77361e3c2d9c6a67f1dd3ff77e6d799f0071bfdfb810ccbc354fb3355b4d2ee86c3869d19a56db14659e12800b5e6b5ced0bdf19e39" },
                { "xh", "501686b2d921d1356a7cf2b0e47a851eaeb7f83b0001370ccd9d53939ab7a40ce54a6f35e64cc167a28afcf9083f0ece35ac732659e6bb51f0ed6f09d6e1a0e2" },
                { "zh-CN", "9772f409be1dcbf91dac32102189113a001c0b5b36e661fc6055707700cca72be8fbd6e49004253eb4f18c262df1cccff5132df94b8e659f599ea7aace9be407" },
                { "zh-TW", "d315de7e37d7ea506ea5c68d59f2e947dd4f52d173af5af3f68cf47fb69450f7da9f2df831048b52887ce65dc6483fbd8bdbe70ac1d1cbe6e951ff67c1b60391" }
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
