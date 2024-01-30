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
        private const string currentVersion = "123.0b4";

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
            // https://ftp.mozilla.org/pub/devedition/releases/123.0b4/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "68d8d34cf91e947ea08c1b2c9735aaab2633ce71cb1e26bba71d260220cff2141dc5bf0e682d0b4c54e59ed471d7c9fd4ee5d5f1491fbfb0bba8fa75ce0bb1b9" },
                { "af", "f58e39f6d2b28e19d82d88251467e768ca7a46a77a8eb64074ac03004a4ac0915f6911f11bfb6800b6b154e7290828fef25c94b788a3c5a3ea9e004a8b472535" },
                { "an", "800ca4827a86340ca630bcf385eff5b485049427e273f55668b2ef7fd71dee5dbd92492767ca5ed5a887f6c0142cf661a0941885956dfce13cd8300786deb8ca" },
                { "ar", "4380c0f8b907a131f32ebe1123cc0f74d2e880e65b37b2eec29f349a4fc2cd3a9048233d6c9a37a4682e01f696f75f03e04ca79d9d767665477a50eebfaf316f" },
                { "ast", "abcef0becdc44ad70792b9f0a99322afa3a1da98a08f9c047d21a0f877f9b2707af6c316893c60210637127f6aed237b22ceae4731acb689423297e9bbe280ef" },
                { "az", "026141c9db357a06ad95e81792338001d57f410701c3e54c934baad551f20b16d5b9bf3efefdc7a5c9ac7a182c2a540d8b41e6eec21b465b2e4c1b4c704a33eb" },
                { "be", "89a798eb61b81db19277b1fe1b6fad26bc6a1a15e307461788560ab2fd9d9a82215fb38cf6ff388983069f1b9295b0cd91ed1bc65a359ec5a63eda33799a325a" },
                { "bg", "390a28a8d712e0da4b3897e8e243c63681c4142796d986e644d50914a08a8fab2456fc2c803fdd2f326bd171baf4562ab13ff49e2d694cc277373ee16b2f6758" },
                { "bn", "d43eb1757c17f78da37885439932c9453bc14f6f562a38a9db648b133c1bb539d2607842d692cbe3e6a2d067cf9dc2201ea9ee24e6cb17e5b8e4e1e18d4d0a14" },
                { "br", "8a7dce37e41948b3ce7eed9b00440c055fadce2b175c80097f04299ef6a70529d9647a4b51bb15f66456a30e7759ecc9411740e7b96152061d2db809d1ef53b5" },
                { "bs", "736c59f02ceda4422d55ceed8b8afa7913dc7dcdd810ea31d7ca865138815b04795064c24df0ffebc6861c03d33bdf122194e732e15a289687b0797c797755a1" },
                { "ca", "22c3ad36bd7d6c5e0819f57f9b9594ca1b90549b3048c4e8a9cacba1ffd29736daea599bd9d868d3841c219d9d66dec456395a15a8e49593c9313755768a6e87" },
                { "cak", "dcd5b48e14f5512e5fb5998ff7b7fb92728191164457539ac3e23a164a05ecaba1fdba0d6beaa6fe38f98c07cb1d01ac68a7bcfdeb4403bd919ba3a7334837c0" },
                { "cs", "45073ba47789f0c97b342d7c94035d7a0dbb6a93b109a86802a2d8fc18de14a9a8310bee39152d88280f9634181aaa5146e02c441ced569469152bc7597b95ba" },
                { "cy", "7a5be359f5aea7640e91622c8d6cf641497514669d65a6dfb041bec6d87d1a4eb6324a59132d7893692c1f8d63cdb9e27bac66a517185a9e7bd4afc601f35034" },
                { "da", "cf3d24a451ef6dcbec138253cb6b367e02ff6075f8255707319117034447f9dfc0d4d0866ce698c6acb4b236fbaf1466248e98b964553f51f4472a39968e4436" },
                { "de", "221eebfa9465b7f9cbb412fa5ff2b314e1a8b495ce4aa4cc15f4bde9b3c232c55f85f4dd7ca365f4ce753c8ed7366828b216e78ca5feff149a4e0cbe4186093f" },
                { "dsb", "83aaaa6cf9181f6744e4d8888d3802c4fc360f456105966a0fcde9c2e2d066c6165159abecf6d10d51eeb4bb7cb4572c6a3a8f1b03875b4d215cd8abc829ec87" },
                { "el", "6ab9f7d5d8e5c345727ef374415f6fca8a961cc1cf0c115106fd5d567804c0219720de68b339ffe9bd125802e046c14ca58cd6dfa74a54a987d2c85ff6b453f0" },
                { "en-CA", "e2438fa0962a5dc94bf225968e21abf6c1645f35dd9a2152d47e0d5ad3e7bdf776176b21aa065f582faf45abfd799294fa4b01016fab0e63604d62158f696499" },
                { "en-GB", "2c93ab333f7430edcc6f72d4c83fd44b87183b65a7b8b0ad25e81cbcdfd433d11a9c5f615caf5aae9c17a6e2d7c933d355ab8daf92adb4f01d1ff42bf40a3a32" },
                { "en-US", "18ac122708fbb35dfb0eee07cbb61f49620f5c1b6bb15b6e4b6f05bfffdf51958ab0210924ba3b556a9fa8a07a84ba6aea5629458a5f7b2c9560f6b53b85a362" },
                { "eo", "4effd95dd55cb035b46149ebf9343e3007dfeefe570e8bc38a80e6e3b78d98118938e7c1d9cb686bab9bbab24fe925b1953419a409f8a9ae61dadf07dfa91027" },
                { "es-AR", "a234221203ef256d45918018fdef49a323555e759bd08648185e06d9f0dd2038975f625203160e5032904926ba21dd702a6ad2d54c138787c20894ce8a5f2c35" },
                { "es-CL", "bf720692b9fdae6fd16426970569338a48b0a034c91b2709684ff173b5c0ae905b9d8b8a448dd3086bba1ef004f8a773704df5882eb443a6065506d2a0772eac" },
                { "es-ES", "fc733169fbec87e2d4c7663620926b5a121d957696ab8fdd2b3009dbbb2fa4db7c4b4c4f6171873c7e3de993f037003ddeba08d8f6aabb3720417e759b29d26c" },
                { "es-MX", "45fdd6ac6d086b74ae9894509157d65646d02dc050cc13825c7f840e0e967fe699f32f47d1b1e24addae8f50d13f95e97f49e7aa42457b5802daddbcff507f10" },
                { "et", "03dda64a47b94a5a2da9b73506d5383a46f9e1b8699ea46cb55da08e6f911d5f73bc788b5c049fda10aa7b5cc56a6d6ce165814ee29e6eb6136fac5930758112" },
                { "eu", "aa94c0189d1273486591e5f14043d5bedb99c8037351f220b6dfc189e37d11ef96fa53ffb7462b3caee9abf1ab59cf11d726fb1403102553c1fbe8b4772e02aa" },
                { "fa", "a36d999477b9823b12d5a01968de4915ec954d43882c5469cb2fbf6ce3527c1e9cce37956107176e68115536e9224b01a039fe804e7b44ea20417b382ba1a064" },
                { "ff", "fddbb7de6530dafb37373d108fb516ef59a0fcc56b1d847cae363ddd1a192f14310371728f4525eb374f331ba00813e13ae5b6601cc08c0770a27ce16597ad92" },
                { "fi", "4b0f94b43b9b3d7a6a38808e04b53c1c54d0556e017500a80851d989002061affde215b77f5252f816b546c49061fede1f586d0392354431330d36426f5a66b0" },
                { "fr", "dd5e084127f3126cadd4b88d55f506f4fdce2738f9be46c7ffb4e00807473fba063012be52e49b6e5f018b107710af60b5e87a53aa6114dc5e09b1ecc7774443" },
                { "fur", "2a6f9e422142ea08db283e5b8bcaa3a805a7d7f6c53479f490d2284b2985508cc9cda32c3285be51633341b160ef84271520afea8e63406c67be6085d72075e1" },
                { "fy-NL", "8607b3d5906c13c74faa37aea2d508a62ab22ea100f18c86f913722eebceb915587390414c23e0df4c59389218c5e40c3e9699d6cc506222f7839ae438c6394b" },
                { "ga-IE", "b42cb1f83bb6214c2437ff97161b37fd174e984ddc40fc99d68635700bae7b2ff58e5c68a86587d2ddbf4606649ac7353e2ac315de7ffb4662f448c9c5cc94a3" },
                { "gd", "14fd0eb2e94037b3f7fe59aa450af1318a186b83cf14e0be0f5c83b8bdc018858573c712d79d8468e980d2f4bd8a7643be8ac581237b914c74854dffb47764c1" },
                { "gl", "5ae3a2d280c52919b1c15565a3a0b5d35abbbb72d700905e0877a4ed55ceabfd58363068f0a0ec3f6c42ac8a28d575846344b771e8fba0e8cbdd11fa116450d4" },
                { "gn", "592618bb91dc5ebd786a7ae09c8ec25a1d6d3c5aa38f9792e692052fd1c513406e7c72583353bce1729adfe80279cd39f1f1dcbdcb2849216ac66b4c80125441" },
                { "gu-IN", "42bae2ce4c44a6d37547be55ef153585cf3fde924fe0a23bc139500719247ad9172bf37c8952d2e2a57ca51deabc57668841014dd4819d712cdaf4b9827ba6bd" },
                { "he", "6cd461ba07e0f029ac2cf6c237e79ac4b7a25be950ea0e44376a68d5d297bbd68bd75b36621404a36bbe0dcbd1c481f18e6f3db93350a1487a9966c189478e05" },
                { "hi-IN", "b943c2898d50eec6e759a81361f330c20ac9a8644404bb11544cedf4c6d063c597c468ce2e9fabf963c51ee77bcb0ffe365290ad97daad3c03bff4b6408df2e0" },
                { "hr", "a9579b36335d1f3df7f98342a68662ed9e619e4ba4d6b4644254e7c72da0ba90bcb96a6e234e99f98b02777b52c50d8cbbcc407e6041f0698b3df68b1fac2288" },
                { "hsb", "3bae09f4d65fae51266d1a43c9cb63abab4cd438e3fd4a89d64b7aacf429c6cd4ff1bdde2c7344b0927ea1e0af87871c603af5736629bd4f97166b598640a377" },
                { "hu", "3dfd3a003710f15c00c5d2e32ea055bf6a93e345782d22f5ec9420ca76c8af6a6278cf01f727287efc384f1a8bbdc338eb3a234426367aea387b7d14fb7dc02b" },
                { "hy-AM", "3608f783ffeb5fe9f089791df9849e2a36ac0a0eef7dc84d87bec39f25ff73ed0ccb026a19e56f58e47bb0ea559b2523f3c84818de3655557d74ce416a8ea49c" },
                { "ia", "36beafde7383d197f4d97515f2d7871a9fe9543338d3d2c3a4b43abe000b8cda603174a2c95ff949c51b1f513c677c18623cd8b56f5cfa6f8c22fe8ff19eec3c" },
                { "id", "c15cd299f5f5bd44ba0da28ae95434dd9a2272848d83a1053b2d889bc21850abd16655de5d9a30c897c9780a82eb2c865bd49c482f1cd082ef38bdbab28a8eef" },
                { "is", "3b768020fe84e0ede0d8b4f46d417c75c709f2b45c0a8d847a9d8d2d1d8edb1f94474fc1ffdfa6d65ae342a7c0d56a1a6248a86a9666499a29e582a21ad85893" },
                { "it", "391ec3d67bc3c72dd19877a25b40aa91a885a86b1b40bcb154d77d81bb41888748e526b407814c254a9c0977d61b7583def496cde3008a140073ff1bf5fef7f6" },
                { "ja", "26570d26b07971f81d5b64b5235b98d54a667326dbd1a6812db9723304aab2407b7425317a9b1c5296e90397a1efc87cebf05982f544cf0fff6a7b580cab45a5" },
                { "ka", "54c92b97bdc078c0840cf8a12b98ea7733ff5d0beb85d999f33c498dea961be66a349a563ec4da4f3ce80f9d78c21eff50b613493ce2b0b973db592188ab790c" },
                { "kab", "8d815b6feedfa2b7610000dcc9726861fc2c1a6b910452043bc99044d702b9a07090121fc8961ca1fcfb3d0294e696f7ef747abe6964b4e4dc10782c7d7e8365" },
                { "kk", "92242e04b103b895825aa6521dbc5997f1a2db4fd13ba0c17be791ce481f8517ebe9e1bfc2c328624daf547b5d8eabd2f4fc94f047755af9029ba0a6e31ef168" },
                { "km", "6349cce396e64151b057598959080a7f225bc7a3275c8d4a0cb1c123ccd5e851d71200ed6c0d7cb52f4253fc6295e5e3e1503dccfa6ebe3a3df3f2181508e1e9" },
                { "kn", "8f8aa6de4a98207494e89fc50809556a3f2a2d9b45ad76d7083b43f58cfaa12850b366df682a2513b779afdbac4cb5b7511fc0ddac35bfb8ecc0b982e9eac08e" },
                { "ko", "aad6a1866611ad9984789a459be113060dd3312110411e77d93f6d22e89a45e3e979f4f8c81851c258efdbc4439849e2d3400777c492f6c0ed808b4b797541c2" },
                { "lij", "d438a31ca95d67c619cdbd6bbd72b0045449e01b7f2e157411ef876ca782d338d5ec06a90851a919647f44a4ae178083172c400ee6e961a6baa1806d99878a7e" },
                { "lt", "4743bf373472e9dd6d30b0af076aee43f99d1abdf74d4a233d3a70b1d9a169f73ed15dcaaf9fdfc87f03b848ecc4cf7115f356a9e170fd6a2ae8f83fa4ca3164" },
                { "lv", "097bef08d898053c30318679f7d022a8c306598d21c1725e50378e50f380175dbee94dd4935016f2c3db4f3b61660afc371cda0603d5635d2092c5bf7feb3e08" },
                { "mk", "0a6646921ceffc3521f8fec112e49f285dd40128c80858d8213d3e5524b4f13118f924fd2b3566196b04dc78cc3a68938b659d22a327ef944800dab87f75add9" },
                { "mr", "fc1d5ff7b0d1987d05f6735c611cbfa234a4d2372ccf8c9b610c66b7a36f69fadb0ddeec92fe0c752ab7efd94b183a8d4dc7868128836d0e9fb4e281e6b9ee5c" },
                { "ms", "f4c22adeb8a6786d7180c0b8ea06c3c1c4c26429bf8a1dd79ff56a24ebdd919b02829af20a3cb28869e1a38d2a4b65f08263d0f01ff2410950379cf906b7ed07" },
                { "my", "770814c8b064e1af1595f0c153339e656037aaf7824267df560217f794a4869b0d9a63275d8fabcc6630d8048e2e5b8b685f8764bc7f75939cc0738b4a3cd118" },
                { "nb-NO", "07db0951287307b55cef48cda6550a40786d26977e71dfec13f6b5d777e1f8a8ecd6f22f432fb4933e3102155990ed3dd925fc161e0255100d3982e17cb695bb" },
                { "ne-NP", "5cf6dfeebf8d83622d385cc81ef6dc7a62ae04170d75ecaeb24af1a271e899977940caa7deabaeb9094e3b7696ad6cb321698e7588e2c24281e76add1cb257ca" },
                { "nl", "41d0c07a46af509c74003ca4cfa8164389fca9b6422be8c73d7a5baeeeb95cc010b8255ac4c9fbf41de89bd9616a89d26bebcce399b84351d7b7d9aa4f7cf800" },
                { "nn-NO", "9e0f4a5eca9e089221468803749d12c4357ef496aac8e22a12579af70d385e8455313fffc86e45929d7a1b05c864e95a0fa984b932ae15d21d8109c095f8559e" },
                { "oc", "8d229a9b4ec6daf1b764163a4631db686ee7a56250b1d83e82e2a85859e9a8504688a09cd8dd16bf678e6e40c92afa8af76494ef35e2ed1b65b01952bcbe771e" },
                { "pa-IN", "b7181023cec4026688636be888f3c5ab8baa9aa65a07d33cefcea777068ccab0467a343daca16d1cf8ed0f2abe32fcbdadd300783689ee47ef11634858a2f2cf" },
                { "pl", "8503735a2d0d0f918c6dad628b67a7ec8da3b18178cb23804491594a58d127e09a1fb08c849dd3a524890136a8974037bc555a22039973b93efe7a02166fb218" },
                { "pt-BR", "0faf58e37c95038e54d7669b38aabcdb6fc2c76016de4898cb56a1841aefa77ba4ac32e8ef8645fc8ef206e0c0c5c5e51ca665afeb10507dda51db4daf1e6e50" },
                { "pt-PT", "52913f6cd30fff8f23e4cd3e632de5c61dbca34c4582ccd601dbf4e97ed651e2a53e025a50f697c0e5e4de47575a3d961870c25d24d846d15be5c61509d777eb" },
                { "rm", "a247710854f1b74e538c46d08d98a8de4dda21fa2a2ca426e3d02dfcc771fd02e81ee3322cffe015044075cbf024d663aae6db3f8586df137e3c350542a04237" },
                { "ro", "79fdd52eb38bf6227ae0ecbc60d491313ad8cad2caae9be0f089f6d108931dc7d58acca0b3e9a01d020d91535b365e8cb8c8f25e52db3ed31d0bdb51f0184282" },
                { "ru", "4741169e3fd2ede6813204d3e09f055fc842121f37589fcfa84a125ddea924f21b330da2faecd3775e362878c16317f6ae5d81bd9b595498aa74997570f158a6" },
                { "sat", "ddf5cda1bc02abec69569b08365458b03cc4faa4780e2945a74a97da1d1c489a507495a396e87101eb5fc355cca6f616ab29b54bfa601119c8fc04d4c0566fd2" },
                { "sc", "14a5d6fa104d9ab77b46cb1362c5411116e7232cc4e04949c7041e6bad05fc5c70d55491ebd00271fe85fc81642d7ec7645c0b453f8f08c415136f8335f2acde" },
                { "sco", "9ad92f33f1436e7ca25fad2c8de8ceffa4c49b85bc474e3a376713656381453a545722d4f217924a91d35b1e70285f0e11c7c56bc326b1bbae65e20c5f5c2fec" },
                { "si", "005aac938ff7d313494b7b08eee8b7ebc8fe5fb0346f197476a40c18cff7718e3d4322c6ed4c5e9eadd40d8582063474fb7cd22981b9955f607cc19eb91417f7" },
                { "sk", "64d8eb44069ec0b0daa2ab88948ec80e799db28b19c5691e19eeee13b969a6c031f15cb1d6922646891f240af0a7f81ec65a35472e6b843767b3a86f3d969666" },
                { "sl", "d58a65e90bae7d95a8e93b1e368c14111fd62787663e2185ee83e2fe44c47eedd210effa310127b24d267ecbcbaca75f023b761bd8fc53ced1dc205c589fe46e" },
                { "son", "9ed9c61c717925ac9f01e13c4dd8056e318d474531ab367112406efa05a1312e913f6027ea16ddfd22c03a1e78e703ecc4ac4261aee361e3363c170c9801eda2" },
                { "sq", "e12287d4d1a1d013e6a74f45027f864ebf4b620dfbbaa67216ce7d508d3ee413b23c7644eb865e9de62eb983581d2cdce8ea7feab405221373654e079054dfd7" },
                { "sr", "289e0f410fe4bad77bbdbfaae251932905f722dd7f6505f8182c2b84eba4e4e946f659fd9a27861f183eb67e183838eec171bcf12203a337c1251e7b9d46e245" },
                { "sv-SE", "f578311d612c8b6ddf5c745f5d24d400639834c53318cefbec3bfa08e02d9fcf5b28616bdefcf3d33bc3a27442f3166e97393136cacc848ffe9087c9842a2327" },
                { "szl", "60ffe9419022d4430b00f96222d7bc0afb3a4ad2f77c96092ec2c5671b8a975d722ee97f597425e2ff2a69dabfdd7a1a616cf61ec412b7386f7daec303089d86" },
                { "ta", "22ee322bf02d8ab834f67ae9e6a70a405989a7501ba119db50f0fc62143721f9e729a80ebc8e5678237282857fbbb39555a65ba95bbe7c1ab4493a33dd6f40bb" },
                { "te", "ace5d999ced76f3a59b794d6a4f8070cee4d22b7487e2ed693c14144bafcc9a5d52ad117a7bbf611afc476a944d6f3cecd00929aefef141287b219975b989a4f" },
                { "tg", "2f6ca2412aefbfbd46cdb37417ae71e4808b68def9da3ed0e86310d757e79ccb01720216d3386caba1f6a192feaff5deefa55bb80b1cfe7c0d455da71e4efd67" },
                { "th", "b7408bb8565256d22cbb7ea15e170b7a56c853d3763272ec4c64d6229270bfc1578671659ff676502326486ca904647714b29247075d0cc78e216bb9b190d642" },
                { "tl", "d5cca55487344aa62b5e972c63fbdfe13f03847c91bc8607bbb71a578299371f07982391cdaba38dbc972b375dace52774f85ec8b764776da536b65c9bb32492" },
                { "tr", "e84a92d28f5b45dd386d90e9798868f92d69bbdcf170795ab013b383a0904f8cb06fc318084d576f236872320d07dbcecfa0b65e8b10d91cce2c015a21c81a7a" },
                { "trs", "8fd259339e10fdddc4097af46b579bcf4ca3c386365005429cc9382da2b43f0f748f07ddc7114312097c28a9e1e267e10bf01c63935c3e419410d5b47ff2deba" },
                { "uk", "2992b8fa6917a20c9fb363bef54bd9dbafd6897041e737d8bec464b3a2239206046782c0b408f2e65f665225db0306b05af9e14ce52effe90bd2b4fb84c8ebb2" },
                { "ur", "52b160c2ca13e7583f11bd87c3481c28512a5c8eb13b3fa755871d9282437768f43c6f00981f727879c821841b7d0185f00b43ccbbf274ffb3df2c7a5604bf5f" },
                { "uz", "5025cb9ecdbe8c61dd412ae4a17ac26fc3e748912de34062a158214ff4aafceda34e7008ede653addc11046a5e33c90fcb23cd8905d6802794a0c5e94cdfdb30" },
                { "vi", "b38b2bc2bd37ba180744ecdfe98eb3e0875a80845105a0cea724073cfd23ddb380a7d7fe61ab64029f1989dde424ee0466a2c48ce61718e593b5eb76991c2fd5" },
                { "xh", "100cbee16cb86ad22a2c07f79d04d447e23cc7e32d9c59d15d4bdb62eb9e4923fee7369d8cf5afa1621ebfd14f86b56b8ea1a8ca3e8417f67190cee80663b4f4" },
                { "zh-CN", "97f1173b85e4cda5dfeb7b710cfe74de474834f9d908250204c2d062c81050341cb37bd37f5fa3c615cdba41c03dd52e6a472f7794ca305557e552d8615bea31" },
                { "zh-TW", "1918893867977352e432d7828a458ccc4bfdcd3674772e56ab650c29641214768d182af1f9df37cd82b4e833b73a628fef5c0f51528781b7e82c381cf05b28af" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/123.0b4/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "e0422feb6362a7dd3c87b1d6a81e645a919f9830f5eee173911853ae6f509af2389c6956625b96cbaa91dce120c551f9a9f728f49f0ab020804db96629e99179" },
                { "af", "be8dea12ccfaa021d731eab8b9c19817753bf00c0ec219e8c72d580bea296924ca118373dafa327c3576db23518a10253e639df77531e66f9a81eadd77f44340" },
                { "an", "0eab52ff862c99ecdad605c5f6e4b0d6c7fc8bbf4606735bbe7b8767672d50f7a464cc74db77566fe852867d96da5f6e19b11f4b83a83e7cf9a4f815ba371871" },
                { "ar", "11dcd9d60ac8ee087008cf0b51a51a8262836b5663c7d3416df7431adad802300cfdab4bf3234197aef22cbdd7e3a1b9559f94c4249f300d1bf5c7769f1b1de7" },
                { "ast", "389c5e49248d87256e27ee326484abf453e371dbf7a4e0347f00605779629073e5b8347c4d5544df6e9eb0b621ba7ac4494f1167ed4008623e87bf86be065eef" },
                { "az", "f2c1fb726c8bc042fb36c5e2768a548782e872cca9b29fd4b1f183a8fb33e6fd9775fe813dea527551ff8795321134ea0063483d1125d738b4858ad99132f3d5" },
                { "be", "42f006c000f5d588ee8678fd0a86b3f79a0cb25b8a9514fb9bc76d94210f97ee70c5dac80ae39578b5b0f058247a3c0b518e6d2df0f581036923e3f17b68de98" },
                { "bg", "80e4469db8110c2ed395dfdbe611517acfa29f57c8d65b7b76f504f8d499c3befd2bb1b122aa0fda988eb9a8b4c527b959c5942aad2f714e5f3a59497c1b01a1" },
                { "bn", "7d9a1b021ff9f15cc9a3a77dafd15ac305a26ddfb4a611c3a9f5e2e04ea2bf1228ed7f3fa376a3d519ea827f0df12a063d020515fb4d4e97145702b3e9cc887e" },
                { "br", "6a97dda2cafc39bbe8bdf6269952cd120787276d132367bac21656c444e37ccc12f9dae7ba25de91508b4f10fc088d394de191c3bdac0116faff95a9629c2b49" },
                { "bs", "c287925d5d49a56fbb4bf8f079a24a8b51e1e35cbfcb56d2752c08388607d9fb8fdc5a065fed71d9a522f7a1466e6b726636dce53c9db1aaaaa1ecf54df21402" },
                { "ca", "c3279ad5448b9b888f62bf21dedb9b0bfb5a60ce8c9ef68f85761b8a4f935f456d27e9eaa47b7e9a0affc6aeb75add2e4d2e6d4b1f4c3e27c7ba3e6cb770cdc3" },
                { "cak", "89457d5fd9cf3a4d4095eae7c1ef472bcaeda1a1e9624102e4cbd5b3b4c47c1a0532e449fb338dbe41275b14cd16593cede7740c053d3436c78551bc43267fa6" },
                { "cs", "882d58ce7a174bda4b2e27d9430204efc2a8e3682ded056c8f660ed62a4b715c6a4f1c5e797ff6486ee631ee965e8e586f6e3b2b8355b00665500e27879032d1" },
                { "cy", "cceee4925480da7dab47e5924016e628847ce55e13ef52b9d27bfb7c60b5b40c3810a63f2d2fa087f22a7c05db90df35953797bdd4720236055846b1cc6689b7" },
                { "da", "c7efaf83192dfd46f03d3404f4400e6bc92f5a49ff8b206e14f73ce2aa9e9aa92a37b7aec8b676ba61f21ed7557136ce77c994f7778ca6f905032e8c9adc6c13" },
                { "de", "b8024b6493512b926311c790616ac997b4c4b3123da406b4792e68557a6160c084b860bb6f2570456c51295c85fbc77f527a4b81bc6bffc2b22aaebcf22fc833" },
                { "dsb", "1095a0312f43d3268b7cd687bad9ad35506689c4417e8f3870c379f8e0335a5ead6494576054e4037df67f4b04afeb5c5825748644e6222dbb73c68b03cf542a" },
                { "el", "9653c3693659596677aaa24d0d782fb527f4306ec1bfaacc33e4072671b51aa0f466dfeffa26fb7247d14d5b1137a8f51ddc5372b906778a26c09036f999c37b" },
                { "en-CA", "450895242300dafe5bd265dd52f8bc8054f36bb29acbd742b2d16f217367538196d19a6c81772c860de7de2f410f2e7845ee10d4c3d5fd8942f514abe4a177aa" },
                { "en-GB", "bb0912925d53ace37ed27f8a2dcba93b95d9527d6570adaf86a61a182982505b9a81e34f15a7fd3424d7dfadaf1288fbcfdff9f5cfbd7edc5f6dcedea89b6f4b" },
                { "en-US", "4295c9ffacaca8a37117e2f7de341122c067fdc6f2ced282d1cf97c2a3f88211ca1cb8075017fa3dd60f8b4bf720116fc4930f2ad9f8e3886bd995621ee80b37" },
                { "eo", "1af5767e7b47d913a2548a123f0896c948d1d94628042ff70c2f9031354d72b00fb67ad650ee6571ff5776df988cb004330d8443290376a271b9a26a2be86e0e" },
                { "es-AR", "65e44594477bf9734bcdb6907eb74a6cb8509523628c36e9cfc7dd9eb3f9b08030c2e7f363ce1fc3d9a5f2d8c8a001bba654fee466633980417c7dfb91c2ffee" },
                { "es-CL", "9985bcf25b967c75314830bb1352210659d340c21b154ce8178c90af62e4ffcd81b502be2f9b4e2253ee5dba286bfb765fcb8eb093797b070b22a527b5f695f9" },
                { "es-ES", "a622c8cd6a0bc7b6b4e5ccd29a65711d0cc08fd3e24a50198a77c1ce16598d4e56ce78e0a33191d271ea79dd0da9469eac83832b8787f0bc51d49c76dfa8f167" },
                { "es-MX", "f631d42289f504898128e906e2b62f933a5a09a729deee95209ca458378ab8e6cec8cb7dfd5b374e8bf1793f8153f164a22e52457ab0db918b3badb72a4b02ab" },
                { "et", "031d588b1d04627938ff77c6257d0e06fb6c7c6c5f2416558b3652c528a7ecf2c1e6208df4bcbc4a0aacb5149cf6a904d6f297d1668e54511551992b9a4bcf50" },
                { "eu", "633cc470da92e7115336e5244177a3d51fc9a11691f23e671fef1fe7092d78878ec47fe1ddd9d636144e6864f38aad282cc807339e7a13a6a48b3c493e61cc6e" },
                { "fa", "b0bd90e92ee4b9bcd9a72563a044dd689fffcd9bf1c11be6a651024410c98789134868abc321a22f86165265affae3b3842338ef1fdd98d7b4d1fccb8690dea0" },
                { "ff", "194efc1096f22025a351123d0fb27cc9a814fe2c0b2b79754cb2b7ae10def58a5363b7fa8fafc5e52425f33dcca2c72f62fc759295bad7f3d1047a2ce5d67bc7" },
                { "fi", "cb399c57e22857433bfd62a394917e6bb7e9aa280be59e92c041e0a42331da5a1511db5ba44c6d15539b07fe91d1bd23f5db206e1a25d61fbd9a806c9a73e1af" },
                { "fr", "f8f75cc23168ec6ceef8dfd41acdd4ed5db5945170736208d6fc13921d3d5ebc37272e7ca2ca5d4ccf38f68c3e483b8d988a8c5c7ddd760612fd8d9f8c306684" },
                { "fur", "fb5f66386feb1005c1fbb0aa216aa165aacbe27c5fe46b0c54390bbd731700cde421cd98f8f42ac31817d3702a0376d1b046e35517ab72dbbd22baa5d066c6e0" },
                { "fy-NL", "f2a5237ca9ef6ac16277765de9907e573788788e67ca42e8c49627d26e2fc61fd2cb1429c04fade62d295991874a0e9c00d28ea9b2e9091f85c5568f3612d2a8" },
                { "ga-IE", "dc49d47ce4e6762253cf24555eefa8f60b5e7fbb6d7c8209793b1f6def2d4c23367ec12dc8e20b66054b6148c37b5f24493bc326227281668445cf6cac4b8950" },
                { "gd", "73f44ca172733caea6a3bef6a9fdeb924320a15758bc5a3d2a5d6dacbb3f89d878a3dcea77c7d6018280ae387db606a5158d73b7b73f50bd28554fb7afc989d2" },
                { "gl", "53e95797f70f4af97fe8ab37374f3f5e8df40cfa2c053a509f5fe3ba7c4e9768ee1fdf12ce72e5cdfb3c05eedda103de18de8f81fb6eab3630af6f120668c822" },
                { "gn", "e155ab78908cbb4a9b76ee75ea00e6788ab4513c693992b6054905904a8a8656f2f4da6952099294c0dcdce0e18fe42eadfce19d082778894b339aeeb696aca8" },
                { "gu-IN", "99500a8c35d69de566d443f0671c7fcccb596a6438f319315283893513129676ebf52a2bbd55eb25689b889f986fe79103ecea287ed6c32c83eecf4711a740f9" },
                { "he", "e4897a4b282deeae5bbc866ea2178cef1318c67dbacbb160978aac9c65f45b20506cdba40c230519103c82dc4a193f1e2e0b0e47a7f374d562e68f576dc3170c" },
                { "hi-IN", "8251b472e7c52e5ce9ee8314218b7a18745453dcd6ca0ae902bda831314bc71b224dca21282995584323bf7993e540eb1ad6902c1d0ce76ffe711e720073dd82" },
                { "hr", "40f0ebbb54c1fb453d4ea8f5fd73c21eed2fc5da854b6328dbe0f759e6356b51d550a1082aaa2b745d1caf68d7007f18a548e50b4d180b1f84afb3a7eada1614" },
                { "hsb", "8c675e8d1be452049497cad7fcbb3c2342ad267b2f68443b3231cbce6044eedfa324d81705681ed1b9f33f4aa1ebfda218616f012419b9c86175fd5aac8893c2" },
                { "hu", "0d34c5c8b91a0a89ae47fb16953aceaffc8665565da79b2638b8d5bbf0b70ebe0d6a1e8153e08292537c274ebf2d2e0242c0188cf83f39b54154e6db7a775da1" },
                { "hy-AM", "4fbe9d0e0aaf8d2869e07a28bcc3705c51684ddfa7bdec3625c2b858bdbf7c05096158b59ef4bea06aad7fcd0da2186a3e230d83b1d10a3f8b98ac45e2498b04" },
                { "ia", "8e78179e96706f6c297bea57246ea608c600c81a8ccf69781557740e182b5ec1c4ca417aecbb29a68edd11ad30f9b5beb6cb6c043056e74864fc1769b4e9c200" },
                { "id", "ce1f1aa56917b1f6ba1d60e2fd906345b8927d1f964536fa742d120533357df2d8dbde3bf06d0760edef13056dfe90b4e72ab83ed1f75b54d995fdbbf8ae472a" },
                { "is", "eb827908e7a433a173118ab87a5756cecdc69283b4f5276c044d4c9f7a90af692e529802e8a1860f42595220c1b947d4ebc2b3a8ec357ba183473c8e8bfe0968" },
                { "it", "5164c4d2e1d90b4dc27f0853a259175a9a0f00fb446836a11482198339fa72a8be1ec89c3949f816ce392c7ae7d621967ef43790c93b340af6b3a51fcb2cd6af" },
                { "ja", "5180732150915b2795319f4256337f3d77c68fcce665c51093317ade602c35989ddeec0fc3ecdd86d2e75570cfa34c9c4cedea1db0e68aa62f5f0d139211ff65" },
                { "ka", "7af6bbd813467239f52453025606445b40a74b8b82c46235c347e116f8b095e128c26a168829f3d944e60af8be070ddfe02102bd8ac9b339175dbfc3671c9e43" },
                { "kab", "f113b6a8a171e510bf481d11d09458c3d8c763823ba7a0f971da420bb798fd783da87ad14de1c9945a08518cea291c02677570d64172b0ab9e9c40ea3835355f" },
                { "kk", "b6d329e258557678361d1e273a67287a77eace0df421ae51d119f2426f5e62fe625782ada0bf29114d33ded70cc16f609cad60230e43f58c1bf349a3cf522eee" },
                { "km", "667b668dcee83cadc150d01f00f5154944ec23bc496e14c05b5b605ee3a82fa43c1a67c1aca4ab78f6c672fe4ced471f6dc586732f36783440eb9af6b4e09775" },
                { "kn", "9d44149b7ef7e2117dbf9e1ef636ac3f13547a083f63f2866c9824c6d1980749d70f2c667e9796c89691ef068c5884d2b07b1918467201b47addd3fc8a56a7dd" },
                { "ko", "73f4c13fe5dc6c7f12efa82a343515db864f71d1bda01d289af3f31fdff64b95b5078ed590be79664a0056778ac2dd5872883a3a1e12de5d24a90beb08f39ed1" },
                { "lij", "f795a362f659fb29a58e92ed7bdad41d2cee14d5663d1ca7d17de87bcb75750ce660696fff9ab3e5edb6cc01685afb32e5c52afa13ba3989ac87b56a8b203ff0" },
                { "lt", "132296fc0b8915d2d705ba28d9ddcf572b11bece3db98471b58c48e6565913fb639d0d11fd8fead815f5599fff4888f1eccd530f787143906d625941e803832a" },
                { "lv", "49df777233fe604b20b7e8e274142772164dbf23abaeb6aac623bf7dbfd4b62b2b71c8809c656c14d4f11f914593fd1ee56ba1fcaa39aa14bb90d748c26c4150" },
                { "mk", "eda7737d0bc6c46de8cc0cf46ae75d4a628a3b9d89343921e963e4cf92e7483dcc9e566e42a4c6b02a309d8f42c7993e0182fc23dca2d2172215d6c15863a70b" },
                { "mr", "4ca0bb9aa3b695ac269ba1a2513ffc3027a13e4e5cb3ee6b7a5064b81b40823f92c3d3a06ebf63b1f6e8151fb04ff12b04d51622f77ba5ca38481fa506959ccd" },
                { "ms", "312ac92e9b651f4d2845f623e801c4b79f38474e0bf8fe20097458c4912917b2bfaf372e7e49c7750cc83a59476ebeb2e24536dfb09542bcb06084cbf7dd3198" },
                { "my", "1b6ff858295fef5e309093e025959413e14226ad653aed429fe39c80f2198903bac5ffda26e55d3586f83bc9684384b56dd447e5d01e5d3ddc10dfd489106eca" },
                { "nb-NO", "724d382a76d44226b40ba3f9d66bb6f745d54d4994885c9fddcfd69b8293f2662568d0c430c45a3c8ad92cc6846c16a1f23fe4ba4eba79cab918494cb646b09f" },
                { "ne-NP", "76061502d51df3211dc322d1b4b18869a669630f38e9460804ab293d2e5cb6e3459eeff10e1233ddfe12d110deeb23a9f1b67c297cf95c7a661090b531da48d7" },
                { "nl", "21634f42e478ff60ea5de69624a02a42438810b08049d10188c97d110204bd73496e2a509c65320462550e39e630db18cf1f75d29c0982ed693180d82ac3d52b" },
                { "nn-NO", "b280b1976a95c07e31564385d3e08fd152bac04a71929f496395510c58bfc7766f29e9123f91814429f896b7a3c37612004da2554b75481e185f2f3cc74d8e9d" },
                { "oc", "ae4f8472ff93c65df565b324a0777370981293b3eb74e02661e1996fcc2c0e75ec37868e6aa3e9199e475552bd8a352b13ffb1bddcae5325040fa714bc83999d" },
                { "pa-IN", "d21d8ea4750afce98ae59179420469f6c9e25626ac7d0cd7a45ebb144420f66d5873ef4363945535fe498309254740d0ef44cd245787cbd37e90b4350a1565d6" },
                { "pl", "d6ac0ac3350988a74aa1136eed37f536e962d7703a1df31c9352984a6b5a337c5682610aea8cb630e2b19388a0035f0d604aa6dff044ec2b852927cc17e4bc56" },
                { "pt-BR", "0ba2680bac82ec50fc3c9ba22cf6e684d4474ea0e37d62cba49d1e072fbfa728a716f3f4dc30aa779c44c5604d904a94f7173ec1884d3c665deb4d50606051ae" },
                { "pt-PT", "73bbd02d273c0472e34c116ee7fc0efdfc66578c874d6428c623e921766fc30aadc3e0e72c31b6a53df5b75a013beae4e4c9e784e8d50379163c067c4cf97dc5" },
                { "rm", "f266f68231817dc7f8318fd19a6cb589f8bc14796ae5de6bf34a7328ef4084611dbc4e39234ceee4fe39af7fd1c4f4d633c2b16d34984456395d4d39a0eb5213" },
                { "ro", "96762bcf4e0263e856790a377b9f2043f2867a939f03d08757b7c4ec177c6a45c22ce1cefe0ca21857794873fa57de92d22151c2db97feb0b106180784c7671e" },
                { "ru", "fc077fce6cd7d16959323e16a76385583670825019d7378a46cd09edbbe6dbc9b8813aad32f40ed91c8de6aff15316c815736eede2434b898789cde2875690db" },
                { "sat", "7b5c6f7240b6fb02f6bc25a55f9d9b78e5f6d4d5317a8f5c2bdbd7f4eac02312923dcbb1154992b44f984c8dd6c9e3c3f86fa88b212ffe2ec75d3b947fe2fd93" },
                { "sc", "f9a77e1c18481f4b5fac795a643b6f6a4003c8fd9d9ab691ef854aa9f82aa78e8837b37e5cf7eca09ba7b61f2a253e49cfa4faee1132a4035843fcca04f92fbd" },
                { "sco", "350959560a21e507e38bba5dd2502744e2b306a092d3fe859a6262e11d0556c92195f0ec33b08d7847f746123491eb7946e6415b58e30efbadd4740d3916afe1" },
                { "si", "48c39665838aba55678c766706e498c06f42cd98049aa228aa656f3a095450c0edc92856859d3b574150d5d0de39cc1ebb38e2544ba4a3320c04ab6f7cd1ace5" },
                { "sk", "a0bb71a3eaa0d36ba223729e97a47c8ac88c57b575f5c276269c0766e70aebd3d491a97865227e2b08eb83e46c6af07c9b2b1acbf6ea17c874d6a5d728ef09b3" },
                { "sl", "7a6a2a6fd933fae72cc5163283c33e27b8c7d350be788cbe5052b3f2603eed6d3385d1efc7ca589c0d1debebaf00e71acd073a8159f6023520dad9018e90a15a" },
                { "son", "b08a007b40020a7b81388f8f0721760b9c032f00efe2c7fa426e49612dbf8c37ccc2a2de6ad785d6234c3cf5b0eb95d9ad1faa7528aaa8b4b0de63451e2b8230" },
                { "sq", "a7db9a399f496a15a22136f38628cbb430bf32f131d30101cf9a3693689c2ac8bca092575b420e51bf239f4e4b46dda998ea7abf7f362f0dd19ef74e38893e88" },
                { "sr", "b85bbd09991743fb0c63cb6b8e8f80013a7cfb7f37248afdb24c82006137ffb1f04b8fa1bdfb6744f359132b6c0f548c366f613a8699b7a05865118eb05e1c09" },
                { "sv-SE", "08ed73acd8751c5d0a54dd3f0283fa6ee7e01b396c1710d8860e01c5fc69b4b018cc39db4574d7098ec6d0cfe0684ffe21567484de6e883479d3c720a97624ec" },
                { "szl", "9b2bd2d558f1103b19af438579009faed71fd4412a2d02472974908392542726d452d556cb500a976f23d0364fdea0e2a371765dcd709790392f033f7912b73b" },
                { "ta", "e294a0d35225fc9d54d5d0992db1b7b96b982eadc54a16f24cb49da9cbface0c386a75f5b8b055c1c136bef278e994939258cae426f39d0eb5c6b22041ddc348" },
                { "te", "7f9e915886eda808c5d62db60faa160bd8bc798c8a71528d423cbc07216275adc50d68859314002a5ab3d951ba54d09068f9e8eb91fd326bff0d9d0e7743d88e" },
                { "tg", "fb2cc3328ae936ee39cf707a7214ba716f846aa0aabd83c51c38324e91d9b275a44480ecfce769bc868459ced546734edc8b8319547260147cf8620f08053288" },
                { "th", "34e2179ec6dbfeb9dbe66bbb99bbfac508d4a66f47c27a19024c1fc4745e8764cb063420836d3bbef993b8b99051541a66ad85b0c7041b28d9789addd6964808" },
                { "tl", "00a05302cb0dbb9667a987ddade8f575508fa69cf05c68eb2cd4325b2ba7ec9bb13d7ae868ce6f87d8abe957d8414bd5945aab5a9fb24f4f8088492bc9598b4b" },
                { "tr", "dbfc9a54f4925674c727b395a35f63e9f4e5b1ec6a184f2bdbe682e9977cd5757415701e8c960fda2e9b4c19b3839eb474c394b8e4e18c2cdb071aedd31f65ea" },
                { "trs", "506fab10f524d5e940ee662509144504d949916064e05fee0a1bc1ae91c4dcacd756adc5ee60d4f6ecee8062ca7a3ecb41520a06907494186f28db676e5ce4d7" },
                { "uk", "7dfc871f0ed05c4935e330fcf10b645f371368932e71fa403c396503f0c76635f57df360fcc7d2082324a97e63d65736ed689a099b68c708edb013069741ad2e" },
                { "ur", "da7aa195e69561129c0a195ff1a2833c6077835c2fffb6c8bfd1592ff69f2930a763a099fd499bfc5767d6d58e06abf8d29dc4ce60b37ba1035f579aba4607e7" },
                { "uz", "639f713794147f7045605b3d5d6674a9ea7cdf20fff76f6a2ce0c65a7d24887ed92d31f9c5181f631213dc1ebd023817f445a9a2dfd0fa2f50a4cfaf5e63d544" },
                { "vi", "523039b6f909abd5518e9285a8bb2226a42435d5e1d7714354a317e1a0b926e15f28a595a80f978f11fe1946834380f5f7bb7fc863b61c6dac7ef584e96d4a32" },
                { "xh", "2ff147ec88b90b15ed67ac17301876fb506c0053bdc1d68f9060e649c9b85f58aff8f000419eb57652fbd250cad6400ba3f92693dff2522aa4e76f512011808f" },
                { "zh-CN", "fae946a97a1f404f18d929d083cd19620e83c023d1e35cbf53a28a19ddbc0e2a79daf11786c37e2c13b651f21a3ddb6fe0ba0ae1c6e7480f2c917dc4b657656e" },
                { "zh-TW", "64372ec887661a90a50a131652e8eb6a6b3c6dba1e417a85d11f4c86c399bc1cab0e93b36770b7d52ef8b9be4a24b180ee8f2f11312f46b9f34bbb3794725532" }
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
