﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
using System.Net.Http;
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
            // https://ftp.mozilla.org/pub/firefox/releases/121.0.1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "28e59e671eb576fe330dea60f07fcd2605abe7a33ae0c929c6dd4f2500e472761cb72add4c7e0e695b71fb22860c391807ccbcca8076c5b46c6361adf18643a9" },
                { "af", "57c14a52d4ca946f872b59e5a7c050e7fc3742439eb79c7a2c06a966abfdcc4c83a2ca2a5a51f5b98fbc33001fa9658c1d6611a341649335089b4da91b677196" },
                { "an", "a249cdcd05df99f6cf4668a82be8e4bf2f56edac23d4363ea53c13d7464d722c2b02ee75f8ac014412a6925784517e3de71fa90c2e62c05ee38bffd9f4eb74dd" },
                { "ar", "5e9e6a933edd4ba3145dbf0dd803d6fbfa4a526fd70ea09d107ec565ca34718e97c219b87ceec94f7845c450b85f0992e41f2629963896e047c1997c9b465c85" },
                { "ast", "f5291de5e474aca1cfc78686417a0808348a64780cee3ec469addba8f88983a506acc0b4fa4174ccc1d79a5d9bcfc603e597152153865e85e77bace1131a825d" },
                { "az", "c67047fde61abe9bdae282770fef97d97e93c7e31bcfff0baa86dbb854b124c23391d69df8882f5796c87e2cd8fb3dc8ef4b9dada0a84dab2854d806216fc8bb" },
                { "be", "650fe4fc5bdb6877b14798c4b33ccebf908de6c89f010107342aa3b33c56c1a3cf6ed54f01a1d90cfb2534a7f48f2f750b74ff25a0a89adda947db434e825274" },
                { "bg", "b43087ca74426263a8efd9ab4e2d36232fd10d83fd4e2fa4ab135cb86283e6629cf76224d9cf7167d7c451b1a6b173a8980dd107d1f2c49e012bd9fc235055d1" },
                { "bn", "6a714f613857f0d79167823edb55634ff85038aa74f37500ac0d4b29baa2b5d894d8e2ac1da10c5bc29c69d3bb986e0af98e19e10b4fae91bd8d66aa2257d72e" },
                { "br", "1bc9d5daa446f27bf89fcd85a7632daccf3bb79d8c248229a4a74326bab985ba4d039caa7a7bf4443ca731a39e4bced24965841eb418faadf95a4dd4c8fb58f7" },
                { "bs", "089a81c8adea9153f58cc9eed096b1eff76528337b3ef37211dcc767d96f4a78374a8c6a5293d4e8fe70128ad5b000e6c99949dcc065c4a6b00bb3629a54504f" },
                { "ca", "fe352a7bcde833a501ba4d9444f30b7cdd2c2d991eac9c11a30543d5dfc60915493a475565856ba9cc9a200f1dd6f8e84391df8357f220da09093ee039711a6c" },
                { "cak", "46646bb7bfad71fba4f78eea323646252141003e8d5ba968c839976eacf62682c328057effe46793c520ef5786d5ea2b81bfb5bc7bb15f245fdbec80c6d4e5f3" },
                { "cs", "e129487cd2adcf5a10efcf5d53cc90650123d8e6c21935d8c186c122b93adb283ff31b1062edd9e807301a53689c4c51896e6f9f0d4e8e4e6673a2acb2c48de7" },
                { "cy", "d35c08b52df92f948292190b90a75584e57daa9bd86ad9b354c2761c130691c224634be2ed4c8cfb58aaa6299d025106da1a3225bfb7cfd3be428ccc28142584" },
                { "da", "fff5d1033564727e43f8ed703091e321bba511bafb5397e1b1e6caeba2f37f8e4e5b422287dd5dffea1c92858e3608f8ca9a38a278ede9a964d011dfdcf11c58" },
                { "de", "53e06031357adbebed443a6be67b4187445bcb24e02019dc6b6af9508adb9c466f1f215c141c246fc9e4b63faf6f1495b5b035e0f478df17245d1f18ce8bb834" },
                { "dsb", "0c5bd5de4a1120e79b69bb38f54d7cef05b82f7c24febe1a83769af8aaeaabd51aa0d90dfd14e872b87a61f666f473d2f229aed64b7389b6c699a1a5796d038d" },
                { "el", "5a6da2c9ddafa9399ad519accc6ec87e2d9114f8cf417fa88346393fe85992a7665df38c8c69c85920f1a683c4ea4121b031fa357ee15b3a792f92386f728aa0" },
                { "en-CA", "9d91a3e7ebf380726bd593b683b178c77e79b8406945f63080f5e40e40f3a2810555ec33f3154ee04e04cb5819a69e22c275fb4a847d3b94f91cc55d038effc0" },
                { "en-GB", "ecc05f006e6e967a6e3737406dc74c02deecb9b4cae2171e94aa2c6f98b62d9db722c1583add334927f2b248f1926ca51ceb81c9b21a1b444df0281ee3659c11" },
                { "en-US", "992b50c9714f60531b7b740e29a2ffd3e03dc65955ed0f5966c6a599dfc1660937f897fb3e6f5678f97c10998e2d2dd25f732066ceabb28e12074b8587b56727" },
                { "eo", "c59032e3788e30d75664af51c96c97669868f3fb180f74a5f4abd6154c63a2ba9427733001e4796c9793b4086d33e6040a6355d42c353ef943dfc1404bc875f0" },
                { "es-AR", "9e55d6c7f14092513e8239bef1135497bd2bb8f72dd1aead522c90449f21493bfc982b937de2a2470f8b57f9c7d5113e6be65e07033f23ec8f0ebcf20938f4ec" },
                { "es-CL", "41b341ccd396933098eca0a4f12cb31c57cb1815da3dd0a11889c4c1c3a1c602979109127b2c6c0b82a0e6c868d39eb9532949f4183ac851fc9169881c7093c8" },
                { "es-ES", "0a2437236b31924503ef12b49b35586360cefb280309eeea2327144e28418abd4d183149342f17629ae93db639f37190b564fd79a43884552fae54e4188770b9" },
                { "es-MX", "ca47cb43809554eff9af1c866eaa75a79db5bf72eae834bcc5fc45be409719bc859a0127d0a0b239b2cb33a3993ccfb0d1d9807892147f54e7b4c5bf17e5cb84" },
                { "et", "3a9a9c64ed239064fe6760f4f5fef5f688e93b9682e50b8b5e8f82a8c5e88293b13eeeae82929f3e2d8990b04213b3bb37343e7e3ce5a2e088f52c155a7eb86b" },
                { "eu", "8e78cb1cf74d4f46b3c57d0a8af1e732029fcfbd8da0f80aaba7ec40ac836a65b36b935e6bbe6f42ca4205256035309697df867b2a251d24bb22c407d30c4955" },
                { "fa", "2f0e2e1f3e1b0d478d2deb7e846e7b7f2cf0c02981f0502ea747d14057d51c279ffdd6dde932cf5ba7c52b7985359c12fe3052c91cc875bf468a126bf942161f" },
                { "ff", "7594cd1363b475dc61d457f6e9a1d605482ef8c858fc0bda3379b8773315b31601323002235fbbcee8b3af9c106f2690433f96c15a376a2dca050ae13402a0a3" },
                { "fi", "af0465db8f64b855af3e647974d697bd9c6ef7dae40c6c7672baad4c3f3eaafef324bfd5c6b2eff0fc196671443e68d3f497688891e3d4e6d55d42695760d7b5" },
                { "fr", "0866e4c05abdb4e7c8612f1185b0ab0859786388067ed1a98f8550deb91a04e44ce23173d45aef409be1652412369c725e3e2768963ddbd5f15441c2613a8c62" },
                { "fur", "fb73890fe763fdff5817f19cefd71670bb9f851317f0d448c3b7242522a6a653ae9d6bd8741f006dc388ad1b5a6a11456fb4be6c680ebfffa778b00f30b15d43" },
                { "fy-NL", "746d5180a07f7fb522800268fed1abd9137c6906c38e007616eb35fb5faf82a6c776789acb4351e5abd52dac22abb264cf600a47cd3dfda4b2ce304dc5390739" },
                { "ga-IE", "f38462dbe3954612d2625044d858a31916cd54accdafb3a669d69854dd37a4a38b8b4b19298093a7d602414c64eab2d7245ee771144b35fed91038b98c65a91a" },
                { "gd", "34b0e3431a697e98206d49fc964502ba619daeae21cfb94aee556d88dc004cb7c10c8a20b0d078c903e1b74656ace5cd2070c723ad2ad7087f9555166e01e219" },
                { "gl", "4bc544ef606698dcc4faf60661564f8f3596c40e3d256d87fbaa0e4152248e43fd24e3ecf12e0c158639a687028e4eba3f4f687e453eab1ca07d67658e59606a" },
                { "gn", "7b33e98f5b9b7809f1aa1c746aef9ac1d46ccb5b5d83bdf9b4049d529612674a3b5d36aea063ddbc587390a666412277f2fdcf0e70fe9c8e840ece227f1b5bc4" },
                { "gu-IN", "f92cdfd53818ce8ae4fa0f15e43650d6d8fe90ae3b09fa3a78c3212ab8233ecf639e6f1ee7d11a8ce2e124c2ec840ab254909a6f0bd4f42b49c6bdc6169613f7" },
                { "he", "f9eb7156397da32aa6b18dfc06c808d4b8b87ad922861749e82953214c8e587493a79d052e4e3065761951db74de964d5e4ef4b76b5f18db756983fcea93b6a9" },
                { "hi-IN", "03348a34c295e515ebfb1c43deb6fa71faa5d4c8ff1d00a08edf1a9c04dc23ed3be1cdcc4b653dfab14601fbab4cf30a1bd63b8ae9fe6e23a8420e105f462a3e" },
                { "hr", "b7324b9a82ea53263e4e5004025c5dbcfd89706c9707d63ef864d63f6fba6b2120d5e3c10ee08eac0b3c1e6b1bd7f4c639c5431081c37eed4a528daaa059e383" },
                { "hsb", "008ad671a2d96c10ab92e7d16d61cfc99ad03f42a9a1deb97d1c8e9a047f84bc7f30bc52491cd226103ddc4db3f48f820f4c55bb5d41b9cabef1204e8b44043f" },
                { "hu", "68d150d4770fbe98e85522468206fc5b8ab1dac95338b9bf42bfd78d90da15c601cb188ac1cbef2070136252e3e21e6ac581f67206def4fa6c640dcce5c236ce" },
                { "hy-AM", "015e10edb9525354ad6fea7bf659d5629076b68e37f0976ee26d514aeb007cb0a449d8cccf36315db4a8ff0db045c647a9415c9cd55833607ba913f54d674ad4" },
                { "ia", "fbb458924f0830d3aaf74bb675d3dc3198f21d16e0fe2aea6e9f862757b6ac33914f66f0dfeaaf17325293ccd545bbce3e7b429e3a9590029f79c647af5e6105" },
                { "id", "eab42e54942df3190552681363fc4f47e8e82d45e202c3d128d99a90a9e0f64883004a5f1a0031cd443e6b9dc28961aeeaee175aec0d8c96aa181e1415f9c30c" },
                { "is", "dc68d2f9439bd60587928ff8b711fb975f8feb8e0b00f47d90314b1c46514d571f9a96905b247af0dfc87d6cc54f7c57b90839566b4f0256a16a31a243b451c9" },
                { "it", "6f2bc7facbec1554c1588211c24a7181ef75dbd1cef8ccd3ec61539b702b4e9d7996a4e930e08509fd98f091ec3acb1016b3ab3c60de5d05dc495115bde85117" },
                { "ja", "3e30fe9670a3b295886663df1567bd5848280aad41ddbc5a8f54f51cfb6fb846bd0a0a79ab7c6d9194710fa1a6882a0d6c1d7dbeadf19e33745d86e1f4d4f811" },
                { "ka", "f5d5371ddfb61085ee699e83fc149f96e09ed0a39efbbd5855ae352808218f998c7ca5c4d68640560754092e02708a87a1cdbbbe911739fe961508a6608876f1" },
                { "kab", "91972fc32ce2457e36a2cca8101bde2339391853c2a3e4e8072a25a5315fa49c40c026eb84062c373a25a3a3f69d6c80cec03918f79c0caeed92ffce27812355" },
                { "kk", "9c9818e07cf1f491e72c6cc07566ba303955054c7dbf31d9641c45540d8616d5ba3a5db91a3170c365576c8c151cf51b1df8d464be3fbbb9aa405f6d39e7661e" },
                { "km", "ba541070d387d40fe22262fd3adcb84c3999b1ec38d3bff7be24911ebe6afc4f51092e8bab771a2d3c94af46196d8c450e59ab8c5734e87169c610baaf46d259" },
                { "kn", "97b704e05073908d89d5d4e91974ae2c90bcc3ba1698a7120c2b50438f71e12936dc9715187d74ec67c2ee64f0aa1da171a4f48e467709baab39e1371d2dbcc7" },
                { "ko", "802d4184b013bac103d20dea74168b3f02522425d7b1073ad304479896231c1e8e84e825a34e58d85f8f8ae9a474b1a5e72c3ccfa1c992f988ee51d29461834c" },
                { "lij", "8f77b658def56cc60b4612e263c2f0968ac03c42ff0ae62b10a731809a0102fc86af4892d93b25994b9c123b41ab0fddefa6a341a348af10553e0a4d7f3f1920" },
                { "lt", "c191b85db45d09c4faf8123f44b48700cc734631082323646e28d96146b25d58f035407819ec69c682c4fbb782a6dc35b9efae7cd851701c6468bd1f681deab2" },
                { "lv", "5920ed11a0a64f066ebb83db2115fc713efe25555b90d9163c63457c75ce20cfe83da11544826f7bf3fae8aff4a72dbded3759e84cc8218a9c4a343753942da8" },
                { "mk", "c7a2889a81d51d5243e7504a8dc3d4e11ecd04edef0538f38c369004ed49a766f8c1f5f254e5c09633c555ce66e01ab38b2f74e9d572d9d8e82214a036285cc7" },
                { "mr", "c5049bc91d0fff3f530b143815a5465adc53b1e74227c325e11eee7aaba59ed002551ed35067790be05d154a224fe21922c1597e164614a6a0ce4836e9361dd4" },
                { "ms", "28cc56f8bbe6ee98b4d5b27a3881c73592d3b026097e643cf04277b69d0ad47afaf4b9419f8acfe1d71cb5eb9c554dcb70c81770e2f846ac3581bc699ef88cdf" },
                { "my", "da414e68c3272bda4d50a60f0deeaa706597407a70fe4ae21752bb697e915a37ca68cf22c49ee511f167fad820c7050d98357543840999ce795e151d942ca674" },
                { "nb-NO", "f5190dbf89d05c0a662860b0111b0f3464c45ae2d15af95d723027f58e8fb5de04182555a6763828a0c987249e3a300bc5bb07bc16183cd41e87fd82ee57bf23" },
                { "ne-NP", "3b5822afa99d90ea5046b8fc574ec58a64c5c507f8a20a2bb69b1a4b7eaafe95f8d55a8012723da8198bcb8958ae788a9b2de1d7e53c0178257bf2629d0f51a5" },
                { "nl", "78b6748eedba074a1ce1766461dd0ab385de6cd45b02a7428db6a91bd1bf0899e901395db85c3c873350e8cca10b1928bbd43b021b40fca47a94c2d654eba91b" },
                { "nn-NO", "cc6e55a75a600dab61e456e3d304cabb6d6232d1c0d25e5975fe2f3f85a2f4e406c5cd388f882250d826d187bfeee024edfb8185f9dd19fd8e9304ace8951f2f" },
                { "oc", "c6d4f52eab0cd1e7790a26d059fa6b3ddd5e41d958c230afdea457eaf195f434a084b9e8d9abb9feaa07e38d57f05e44b3da5bb8026676501ef3819a51e807a4" },
                { "pa-IN", "945bc89cc203e0572bb8bbade453cb184d1192b6b9e6e5495a237af059d24863b1d08314edf4cbe2d1544a040a66d86fa1f1b26675d113bac37322b333fe8211" },
                { "pl", "e3abda71d841be66c9452c62831fbbc278ed6c14c5ce8bc71b6ad4864311304856e48a73cabf502788c7c8cd1fca9e18c6c1131f72418947d493e306ce05424b" },
                { "pt-BR", "255e95f2cf0e5269cec15c3aa324c8265c3d4c76f08ea0da05ab54cb1f089d6089761a135a2bfdb5427ecc0ae9f0336546bec783569fcb86f3cd96fc8d9329bd" },
                { "pt-PT", "f150d7c13be888c7b0275cb7fab4f0ad9761bed32984753d41ea51e2d3aaae7ce6e37c7157fcdfc84ed9b6819c3d8d227ab3263ae8c5ffca5c0b494f8b7e6e5b" },
                { "rm", "6be4c5b26343e8f782ed3edbf92a8bab66ddbbd1305363024cc7a84573e7321a37391756d13537a8a57102fadb6dd5729f2252f3064209997876e716da9d2f3b" },
                { "ro", "6341d475e22f1d37950f36289cb0349bcfecc497a5d4bdec19f653c600d30ce546a7fb843da1914bf68055e72eea12af495d97031aef67d5a35361986c97ec12" },
                { "ru", "afa25f95cd9ba76f98aef487ed201fb63c526107654c3293a7c4d620da5f203cd0657749caf43ae672ef7d835f4f89357b2d3f1157e31275afa9c7a5311e3c83" },
                { "sat", "c4e9f6ff3e19f6ad5823810615c7b326bb3d946112db3e728a6efa4fadedf486199eeb77af4c397de6fa7c6a1bf750b1756ff161265b8372de4076d7caf389c0" },
                { "sc", "0e92b5b6d3cdf435a96eda0002feaaf584edd8ec70ba57cf05319cc7b93aae1b7ef0159892ec88f880b8f6044705cee615ba1ae9bddc0e56cd19badfab95e002" },
                { "sco", "e5160449315cf92d1f5bdf9bbd44883775c2284a531b848ad9091f74438994b080142f57d79ecf91a18033066f5754a58af87ac71af82e16b4732db4e16591ab" },
                { "si", "21c484602b820afc21c218b5de1cca137dab5cb57e083f4f6b85f7be9c48a4379bdec020df14374bb38630a971e8f88f9e9e2c350724dc83509dc074d1d9db0f" },
                { "sk", "9babb2b0f304fc0031b10b71059d4358c36596cd418a9a018b99cd7fd71ee7caaa0c64385051b5563d24e953ddf1e27bce7ca73aad74ce5e4a752d120a4fb0e5" },
                { "sl", "ffa0c9fece2ba6ed14e4645271c9331441b0e9e00b1e46993adae8f7e0fcd0801fb55405cc5f4fe7009fa7ab75f31651f3ffb172bbb3b211e2e6fb44b3a220c6" },
                { "son", "2c9269a929872c5e89ac596ba510cd7739bce324b7780e4e1b36ae8170f70f0613301e524c5e85879ebecea8265365658a9160cc7a0015651969425d86cc6a31" },
                { "sq", "042eafa383ee93448a93e419624885caa938ba2af95fa98093e3ed8e0fd6e622d5ec3b2706896f788c6e9ae2fcd73881638ea73b9acf88da47d8a3e55a76daf8" },
                { "sr", "006bf864de54da2922a4530cf53a3b666d23973b89e05ca36eefe5c6073b058d84e41aaa9c2db3b9df1c458d92e5df4a5ab8661b93af77d55061e080b0726942" },
                { "sv-SE", "666195c95bede25b3b6d8667ba2ad8bbd2a32dcf9fed86c44d7eb25ab3da89316832248fd0b77de85bb52cbb0e334e61327915ab7e4f0414e1847eac06cac870" },
                { "szl", "f4def73be02aa8f91d4899750145ef08b136732e549562462ac17ad3886441df392367f821cddcedb1723abf86149cc7bb6870e283a7745f580fbe9b418ddc12" },
                { "ta", "1d218185bbac24d6f17efbcbc3869f46535c747d637553115141d1181a1d9ee0594a6bd8046d0b2fd1e625077bf3c83ffcfd6e4fc254518b36ebe84e9885e407" },
                { "te", "4c558ba79b616959f5d893629ebfc5794f366a6bd18324b447eac5c625dc2737d656e58e92399fbbea5c8207613238c8ed1d468e72b651b40e07ed172356141e" },
                { "tg", "605d679a05ec7cbe14824ff5cd4b0ca47ea1d9f471700c098d629c578eb49982c51a96e742a165ef3f693945bba9e7aa49f9cc1c0225dd61e5b9d5649a3177bb" },
                { "th", "0050b3fbd737741342be3bc5704e8f612c707761a845a2bbbd29177f0a3a1401953d103227bba286dd8169d2488bbf475ebebe3ab90721b0db7f94e5f09c0a3f" },
                { "tl", "583c8058424dc2f29f55484478f91f0e08e9d4629890d5a6f0f0c05b1b942b7e2a785ded415b40773999ee279ffcf5a345acd9f1840dcd2aa38271cc993a3d50" },
                { "tr", "ec0ac9e4c779061ae028597e89ff5889d9d91bdd154f65946c1b0a36d05e35234f87ab8b286d6b8129ff268440970563f2fddf2d8d4b0b884357802bd533f88f" },
                { "trs", "48abcd52df8bed650ea49634b6a24e516746a2df1050857a28441e59d4418afc317948c39fd93e2ff4185379a226414375618e23a9bcc43407238ed5d1acfcdc" },
                { "uk", "a735a2c3c7dcfbcfd59d82c2dd21a3f6eb75a761891c5afdab32758850773bdf02fba8396a492b40676c6f3a06fc9f92cc98d0e81742be6b86a43d8806212f4a" },
                { "ur", "7ed2d280224118100d3aa40eccaf5f105382515a968a575fb76600af3de9c1e92a6c20d2aad110bce09f8e6c855f9647056d7aa648dfd3d015935e07724c4b25" },
                { "uz", "8de342b805d48e593026218a5ab878a83dbd16a6fd2ebc282ceb39590fed56678f55ad85b8c3af4a99e276016f6b75354048d1f0428e5ca08b83ae2286c2a740" },
                { "vi", "2495668a4176032f0bbea2fd2bee20c1a6539831fefbaaf0e2122c3174976faaef0eb946aa12b0ac8c29f68b1e2987125ef6c21b666579a4d27fd603d72eafa8" },
                { "xh", "4de9f710ad1892b067c2af427ec967c78fa7e12a41b069d74aa22893161efc8fc8c94e5dac3cb7386a189113c7a78e508301269e69e653f12f55cdf5253b5483" },
                { "zh-CN", "801d2139c3e078b2872480d4fd6c2fe878910ec0a9fc3d5fc3270cdca9d9d0dcd0717dff9cd11210fb3f0d99052e02faecd7e61cb65bfce49e51958923cb2d57" },
                { "zh-TW", "c44e9d87b712faf269a218c257f64b53b11c91be04816ff4c31cb2657df57969749f66f6d078f3f6a3ce78fad314d529a3f2ce02255f9160d935ce4eb0f0fedb" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/121.0.1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "cc87b7ca550056f4e9c23e61ab6bc8684b98383bbf33477be3c9a492ae4db522f5f1540b164de5b83db466b6d83615cb973129f6b2db6d6b9b881b0fd46e985e" },
                { "af", "a6ae77c7eeb1c1e973c85e4f586dcca86c60e88b3a52aa289a7f3ebcf67639bc8d5f63fb32b54cf73ab1d44dadbfd2c565748c5a87b0577a711af3352da0152b" },
                { "an", "47452090e4c03bc51a610a38411f5612e33d3eda44fad00f3272d87a897062fc11a2aa3555e392cd2f45705b45d73613512abb119da91bb62f7b67726f298126" },
                { "ar", "4be56281129c62ed2a081bd22f8abccb52839cf90d796369c2c664854f94fce40a3f66db11cc62f804faf8b7dc069564e24a1ee873cb278aeeea01d3ef527527" },
                { "ast", "12a0adc336a0ab5ae1728cc9456023a5516e50f70b39ac2f186e74e5cc24391302a8503628ef94eb24e58b4658db0b8290f6bcd31b9a29ee3d41cedeae212a20" },
                { "az", "d5fa2048d75506a12f039f0b3c80d0bee15e69d54e1b9b8e0b4a6370b45486912a6228951702bec5f5c7f14032b3940ab920436159cdd140885001b5f8865e20" },
                { "be", "4b6e2733af2f92d1c73674ba58017adbba14526c00c054144836457340f0c02af97c3e91a35185bdccdf76b2927bacd417d39f13288002f217b40f554d0d8121" },
                { "bg", "6b8df78f99b0ffaf393dbd116a2f3da7c3dba90fb026f62f60edbdaf6e16bcfe5f8cf4278a3f5279f2af93e3252af550e5f3b105a0d7c596f2d983bafae7f877" },
                { "bn", "19c6a0d3fd93804877746a7dfeffaaf0b07617282cea5b77ecb63095bec12f31fc1a4034dd6341a1e7f848b7e0555e6d6d3317afb0fa7eb48133662f6fe09902" },
                { "br", "97ba8a9c640514c0271abaec0d1914ae101cc46ee6906d3aacb4a8c48f3d2f47a4e7ce9baf770fcb72a2ac43d89597210486391eda4b78ccd74e1ea506a34636" },
                { "bs", "5873aecf232d09fa96a24c1a69d5a44ccf57903a9c0fafae8e2d5076dd70a79a57d82e87be86b7b4170f7f25863d426bd938c081b25d6706e40f40ca030caeec" },
                { "ca", "14abe0f41edc41b0d39e2385506f4d863d72b23706e9a99a3b41265402ea3fc9228d72ffdfe4f4793ae5e4939dd8ff02b00adda1462b0d4b0087520b26f80ad6" },
                { "cak", "e96736652a85fa857be27efc695e393aeb2f85d93c61b118d21e5ed7443a2e62249892cebc31e635847d28af73ae0b6cc8d3796f27871ec937dabca3b028f3ca" },
                { "cs", "3194305cc950229fd0732e4df486449401dc28731e074660dd7f9716e5a1fa15317483a922a72048860b03f763b566ba0da0b9200665f50b983c1d6f07a15c4e" },
                { "cy", "ee12c948ff8629df1cca256f08bc7efdc3b3ea97fd131b6b18fd929e4aff6870cb3c52b432159f11c1cffde4bfcd3e378c47ea87d1455d2f99009efe052bd120" },
                { "da", "3842baeb1f76658053969028a6f149fd5a0747ff507c2cc8becf803ea595cfaa2ef171654d04647169493bf4eb8888154f4aa755ee0e1725431e575d0037537b" },
                { "de", "e2ed4c22ee7fe30c452d181602a072fd0304dc58727ef86106ade16bd28bf5fbb04d2a6c261b2e033e987ae0ad1e50769a795191bebd017b8f99667dad1ba102" },
                { "dsb", "b12dfeb923c5b15ce2971b0f8e59e816216db7c2787f7db3a2bc6a44be79d1297202b9618adf436377fa4672bd9a67a5e29910dca979b93c5946b06efc2891ae" },
                { "el", "67e433438752d1515347f5ec1b9851e66f2e355303ed2ad651188e89cc8fe002cef41f92161f10ac5b28e7ddbd6f34ab20130690403a47687a7c65adc52b4d0d" },
                { "en-CA", "896f0b9af9cd961b83394a4ff7b2f1f8a717f6052fddb02a47cd2fe2066f45d7ee64b3f5f89d47f1dad2eb0a04066daa34a07bbb426df98fcc9aefadfe8efebb" },
                { "en-GB", "1c95f1d5e449f08756835a9101b7b40a9cce44544acd346c63be3b3318ad9e9fbd61bcacc787478a251bd6e304d174c7cf3d8ce7fc628701fe59790a6b8c2f8b" },
                { "en-US", "3c02c1ac5bb40b9e9e98d070cf780269cd3d400a9bd21f4ffe79d5586cd2c3f1aa57a6b0c17d674d3e666da8f864cc241493ccde207aff5bfa7555953311d142" },
                { "eo", "8662a2d478a79882c2211c793d832b7ad2be715a85bc4a1c1fffc5ac9846119e15de87af8ff4d4bcf4727e1718964368abdab00d424b004d0876e5400e8f9eb1" },
                { "es-AR", "a572a30099098de4aa74bfdec5e64a7b9aca7f301260f952caea10f61de072029c11f80eccdf34459e2b2527d728773e699721f71fda32ee8498fd1fff341c17" },
                { "es-CL", "d0764a282b19226d348c2173f1591059b1c610d162fada7220512ab2701f4b9ebc908c0905eb6e1b07ebc57da15a7e15ea0cc0a12bc54f5f51bdecaf19284194" },
                { "es-ES", "b123c312d06fee4b5f673f40f8c5631311217825ad140af0211bd56023cc18e79c319ba5b552b064ea76d587b56705ea55cb0b156211e9535a7478c167aabd91" },
                { "es-MX", "63cbe968b31e1f96cb82af2542aa6d9c886a9e6c035242a11f069bd36b5d8bd06985093c1f5f5dce70a2c34c0d52f47763e9ec4f0796bcc50dd48ad3f5755d05" },
                { "et", "21c27ae6ed6d46427ed354fbd871a43b6de4d7babcb86d70315ba6c04ae93d43f95388715bea5094a1310ff86a460e54eb6e9c35c99df74680b7a53aa32bd609" },
                { "eu", "8cd2968010801e55350ece8173cfc7934b3f015ade9a081f8679c2e2e3aa4fafc57e4a5b61aad496c7542556807e0c349ed06b7bdddc7066b1776c36a20c256f" },
                { "fa", "5c434652c0f94dc4b1e95972b6956904a1025ea6e2ef234f9e6da28ab80fb5a476ab753a26d237171b5e1a7c66766f503bb577c41c2788f14f742ed1dad07480" },
                { "ff", "135fe5f8cc0d475f10dfcbe1d3416a7538d9722854da0a59d0f997ebafaeb87b74f978eef85f5b6e634dde41cb5bd348e9b481e9313997def4afc07daa433dcd" },
                { "fi", "186ee9e4febc5c39ddd1a6d1ac131f52300d074e0a93e56552799f2fd7ac6df17fdd903d78b161dfbd64f6f23598fc9b0b600e39008de93aed5c60d4a272552e" },
                { "fr", "83705e0c24a16ed3aad5660a8fb090d989052637365808e79de9fa29c16a2eb54750e990dfbb9d79e732db249e967ba88a15850ec05b8771c55b004d26dc1a6c" },
                { "fur", "6a1a8af7527e804e8e9c514bf8fa9e02f6e466d50286a51d32ab92b150debc13c94a8bcb80caec80352638e6929d88a15e46a6a4a0988f144a6f1d630b3fd116" },
                { "fy-NL", "447fa0b6f7493eb20855027454cf9f57b7e3aa47747da2831b3035cbc47bc7a80b0b3ab869f3df7b4e58281f7db02e6223d1b0c09ab5d0da14599189128dc5be" },
                { "ga-IE", "cc39e23136f41053f4b42e4bf1c8e13fac052ae6392f4b3a229382305b68715dfd6ecbb36884cf424b3251b26c83ddff278919e2b02dfb4b23272a73fbd203a6" },
                { "gd", "6375f891b5407391676fa71e1e620bb9091431e14af12d480aa7632e3b435c4aef3e1981eaaf29745cd4f126d9a8860f19b31c8694f42ea3de60a2981c1e6bdb" },
                { "gl", "572182e92b0c87c3dfdd92c2cf8e02582d3a47c67c2ea95c5e8d9f3dbf99dc7449343459a33ea6389f8c7878ac9f23c92116d6b7ed28ecb6cf9ac31cbbf644d5" },
                { "gn", "3ee8077368be838d93ed671c5e8311f658593edef9a52c58b2d68576b14230b5042905a8512239b31e9ab261904a8ef45badd74c83d8dc6028c12f6c8d747e39" },
                { "gu-IN", "abd9de8665935a577add683b2276dcd684e3ed47dbbc21d65330b45cdc69147e11b38a5b563189ea227578f267c30655663dc4a90757803646dd277d553a4364" },
                { "he", "d16dbae655a5d6f2b86a3b0ca1d1c991d6990206118fa8869548d312b4efb42e1b18b4590945f4eccb43a1f01003faa798a3550ff604b32757245a00aabafb42" },
                { "hi-IN", "b84173a6cc7a737eb050d0ef7339f7381d5c4656262c3b26144b1de0a4379d5c57759b5d24414e87f0fcc6149e8cc9a23b796ab3a361ebce7bb0bdde1594ecc7" },
                { "hr", "0cab1798517cae2c87cc91697b157cd7f37a1fd452604e58cee53a7bdfc9175f8cc0d5232541cab2a11ae78cbcf7d19deb19216d249b9ddc53784d23be444470" },
                { "hsb", "24885a8a594ec2668b261ffba2cdfe886434e88d62e3dfb33ee07ad89a7d1b02b402302f8d9ae50294d0202a6c03fe199fd55c384ae658a7347b0926072ca229" },
                { "hu", "18881556fc2e428a8d5b249abe1bef1d3bdbb0ce692acd41ae6fe7532143914a8ac917f30cabfd0c5f5357cd99e36e29979e024516bc52879c8d7e76cf9044bc" },
                { "hy-AM", "46db6a37788792935b52177bef8985381cdd9c1f34c4fe8b7f12202adf1225aeb790c71ffe82d6f0a6e1b483b7b713fd153b09ab6c049f637cf648db1b47d7ca" },
                { "ia", "e3f1cf9fc33a4d365e9a9fdfcbbebef95abac8a1f4e9d2312f71824fa5888643a7d1f2169a68b3539180ab9b74a0dad35eef811fc3cbddbc0905d819af9b04b3" },
                { "id", "72dc984db93969761a5a279056289e06e7ab853c82cc02559c7e60d5659b0a4cd199d0c9cfd6e0c1bd722005d41a442a402f07e64821ad9a496157abad53b574" },
                { "is", "9b03471e66c5ec2666d888908ee47d0a034eae0b9a560cf4267c9b16924a46d8e940dcef9948025818216733bd2cdd214241e6c0c5d46eb29296729b8d1d4b12" },
                { "it", "1dd37c47c93927bd5f641c2c2189b425d37920304815eaff19ccace8de5b6fbaedeb8cdad9cbaa8867bfc1b625acb23e97203735a66fbbc1d51bb52263ac424b" },
                { "ja", "9a2aea9f965a7bb744496d1ff9a440a29310afb9d7d786dde6cdd19851320dce5e4a8b830cdc1dd3d518409680da067732bbc234fffdc65057498241be70c172" },
                { "ka", "da6bc795d77f5202e08eaa47fbd49c0c6b43d6b539e720f4c74f9313a24dda452b5f4b1eef0ab1252d882546caf5a7d36f384dd1c76235b7128a039c3fdc49fc" },
                { "kab", "96b7735ed50a107fa16cef0fada73e0a0a6add9af26b0ad61bb53a0a453c1224f7b3e091b07102ae2e8d9ee45145985c682378bb88f2db3839b83c80e6956645" },
                { "kk", "ce40dc5789486064e570366c17544c01a2e95adb9fa0f64bae2c13949201711c4f2731398ca601e2da7927117c855f73e6f1e4658e38c33aa4fc2234a1e726a3" },
                { "km", "3f8bac2af49d58fc7bf9af8be6cffaa0345d42c2f23dd2b6a35fe3762061c0e3cdbbff6167af79eb80f7104382ffc47a303c083552b68e56ebf374dc8effda4e" },
                { "kn", "41c4a59f21d9a0c2d1c437858717788b61b7c27f79a4e98e7c7247f10bd534a9ea463695bc00b0d4ef37c8101fd9fc313d5d47c1063f95ea87465eece4f238d9" },
                { "ko", "88520ea91a49b45020ba713137c0203ef020fbec2a720b83c44dff6737272d1065f38b38dba46bfca2921c51e36a7d100d5ea37a5d5b3cb0fb4b43d631c93ed2" },
                { "lij", "4ad0edab96b13a1ba87d374887bc54d9ee7dd91455fe102fa89aff5c70f64ddf89b1be244ddaa4be6892d3d2d4216479d7e78a6a960282a595249e1c8f8b99b5" },
                { "lt", "9b36d689955ebdf09368a80665edb4cdc1a5a56aa995490f0a11c7242e35e1f19f5c3abeafd3dd21de61bf71194d5d110ea67ed69b5b915de9b07b494df4f6e3" },
                { "lv", "a2e9d77b240a838e390378350e3f4a48ab6b4dc2b8d82b824234b9d541395801d74244d3e530d647245895e38b6a6acd0147d2bfd078ad1ac28b063612e5d57d" },
                { "mk", "2b249b92f8f37fe3332d7c5ee5228063cf1c87ee49069fd0c61887279b2a3f84bf7a6043c1776a1405f087e81c7f78229dd7b2ea4a3e4c75fbfd10bd2e0c0fc3" },
                { "mr", "dee5bf4c03d2afcaf6dcc373fc6f42ad9e1e9d22bd4f9b292f7e9a792c485b4aa6c023251a7cd1b1def3a5e4d106728745e9ec6f9b986a0a7e4ad9bc171c2f96" },
                { "ms", "6cde8a2c17b8f7edb5bf034e3ff2633a25801127ec1b9f57ba030cb6b776e7e67248c10bf90b4c1084b30d831c72c6f32967ff50d10973c9a7bcbc158736c348" },
                { "my", "59ee7d87321bf50331b3dd16a998b5c98ec0dad473232bd14dd4fa55d5de1e21931b2c141d8e611283374447199ad984799e132716fb91301d532ca70c29647f" },
                { "nb-NO", "5f3006e4d987df87a2604bffa0e071d9bb2b58607c7e7c01338be71296afadffdcf9ebba5ca00ac5d3d4db999b63f7530a903a2bc9e174d41acc3d42ed54abe3" },
                { "ne-NP", "c6f69157e508a522e070a02ea5471f1d8db9125e071ac456c4e5e4de0dcbd20e4b80f02d63dd4ba58f51bb46156d7fdda04c988aa07545e50af3ef306b879b9a" },
                { "nl", "dfabc33829074a4286b7170f87844714c4f8250e035300c6330dd1c540642371227f161452de87a729ed7c82fb39e72b0b7a5847b4496355b8a8d15e08da3f1e" },
                { "nn-NO", "71a8c3300c8ab94ae88c47e019d14a9f0788b39f932afd18a194e592cb6278c53d29b08b81fc48743404c203262445e97d8c77f2d88fd7d141f86d7d5e644f80" },
                { "oc", "91b1a87cd5799e2380afcf7d344a0ce5342571916d52347e8eb5ff30d2097145400afbf85089a454ac3f898ea3bbba4de804aa2870e5ddfa9987033d6125a401" },
                { "pa-IN", "47fba87c806a19042847c4173960a1c05504e1d53c5ae217e266ce91d8d30fda517e570dba2a9402adaec3f225e175b711ede164655628a771b9468465e29c4e" },
                { "pl", "b42b41ee495bd610707e8c4f19a45e9b09d81dbe700fa20b7e93592bce9630c5dd4769e89a1f918c25cbb617388f133350f2f11c5d2d108feeb266f225ccc408" },
                { "pt-BR", "59fe00180d8f620ac6aed5f44a8bc89cc9fafe573da0c8d85ec41fc6a49113d1ee422687f59c65af3eb4778eea22ffc9675b1a8a852978d8d3ed05baa062b349" },
                { "pt-PT", "c3112f12eac42e2c690ecfd40c390e4c2c3af0033d061f5e1eef47717920cb08deae662a92b6fdfa3cb9b5cee3a5b9d00862fd48ebd56bb6fe017e197642be24" },
                { "rm", "8d0cb5233e775a503d90d0358afd04909b7d3173effe76d86b98a54a699d56363b07f127a9054829460b3e5c4a363c1940594c9e1d44101405a5591e2de7f8ed" },
                { "ro", "f12d9b57533d7c5e1c5da7168848411c76c594b54746b8cb2c877dee53b733966fdc3a3240a3989ae310a935f84229b19549efd74a1a1e6b9b3232899eba2d8f" },
                { "ru", "7096f6996c4faf42183632273e9978d0184f0d10e636aa775bd4ed94b4f0f64c7d50e3498fa4128e3082d40c9173daa17fd9875955059d4119e900cf6b803d49" },
                { "sat", "727a5e4e5fb2f66961238510d5e003c5b84912d0a0997654f3c78049852496efd6af896fb91e13e457f13ff507400a9f371c9d129892c486278af46c45f3982a" },
                { "sc", "9d506e02e1a7d51927892c6f45951714c686641313638c133e581439ac24533e5a5d0b6cfece3c65f84ed677fa6eb8752830e9ceef5662c7d9bc5a026f75f5b3" },
                { "sco", "919d69c5da9b4d7de09aee92bea482fd07bf88558dca3678b6f0278475454c050aca295da88dcd333e224cb2acadbd7f33595aaa8a3ffcae9ac2c3c007a97d34" },
                { "si", "e98371d4f70cc6218b5813fc165285dd9317b3e6a7940a350ba851b3119fd6b0387a297c91a740ba7c3cfc6fc9aae13e23d0030af97993109b826da0f6074305" },
                { "sk", "0cfcb4d66639748e59a5e02ac3441cf2ad956c1f0cc8de9924369c19fccb7ab785bb5ab6ecf9d417823812a3d894a3712411ab36c70fffa9186a99eca97a1763" },
                { "sl", "1a87b2fce2f11f7f361f34d83c1e9bc7def519b8e6234b91e2802d0dc8940ee22251948ab1b02aab7a5a2ef7522ecb6131c969bd16c5480d5ce1f6fd8f82d47b" },
                { "son", "82dcb0ff9ee764d11c0082f3d0fc1287f305d6e811cbe48bd157a8e43c23294bf6cbdd918d56ad36fba2bd19268cb35978630c9cef9ebbeb1132de8f3ff10636" },
                { "sq", "04b83fc3c37fb827fd43b28e4c6e746ec9c2952478f77c8be575125868ed43d0e542f6a19e6f06db3dc4b55ddfe3362d85f362ba463be3305edbc443ba2ff1ec" },
                { "sr", "95078062bc0f159f69b89a2172c08306fbabae8a74883e60042399cf987443a5eaa7c286c298530981ba73b9adaf9d9e59f09e65968828e5dc6b2594205072ea" },
                { "sv-SE", "16027b9a33f172166142c5c44aed242943ade176c35e93e343be8784a876717d23a628e7732f331f8b1858438ad63d587d8f6d1bdd140d59c39764d4511bfc6b" },
                { "szl", "3bae6d25ffeafa2242e1937076e9446b6b3c577e4cf57e18bb011de4b8c42818c8f45a25c7d57ec104d8025440b424a0faf1a6ba7b614559aed459b42677b541" },
                { "ta", "0caadc8b046ef2741e24162d98445cdabd32ced13c8efcc91c3dd37fa257835af91cf2cce5a5f1879ee166469c4a53f067e7db7930759f79dd0f19bd8fc879f4" },
                { "te", "965ca347b3f50d753b040074cd9abf2e003242f06b045ee0649f67286278e4fb4236af52f59528f89763bed3ffcdd46ffedb2ed93f9e45d82d66277a02811e69" },
                { "tg", "99b4c2f92d65ebc992a938559c03483ed723f99b5b52b33b54efb726a5664a10ef42f139bfc2d92a475ab51ff8eac79df3f1f9b237218d24bd30def3e38ac516" },
                { "th", "b184772ead8ecf74a5bef627b76cebcc112f553a747982ae5276b36257815d48889b79470d75339388b580bad0f0c831d3b43b3ff743951c4dc5bbad76f443de" },
                { "tl", "eeccfecd139ac7dcb620522af4da30171af8ec421e4fd429f287559269a301fdf5cacf95efc3bdfa96e76f7e42cbdc42c87ec34a6d1f88642c3ec9688eed08d6" },
                { "tr", "ff51d00f13b1d886383db15a02cef76af5db24095cf978c94a790c76101be03d8ec68e1dc2a9d37c82cec6f1b8380e96c36dc784a4311b06c89f8fe36c0119d8" },
                { "trs", "0a00ec213c199d18249ea07b8e28a91601754d127d8c3da1482c13943263f7bdd1a363365ee377242680ba100cc7dd9f65dc3e4a6bfc4e9b02896bcf061bf76a" },
                { "uk", "1962c1bf94d67fa8d5c313ac922ef55b466f3793fbad3cbef2fcef01a3a54ce027c4a24ea7e1dac38fe1ed2212e7923711467ad7a37cd9c60bbff7ee4b343998" },
                { "ur", "e993d9b3aa83d66d8a1c4f64f6d7d492dbc1af0ae02e0ebca6b6005b29b19d207c5721ba61cc3c5ac562894368fe1125a26f6dac0d7a76b7de5ea78b2d487c27" },
                { "uz", "2f80c0fc8412af5c820bb67dd5d42d12f90e4b882828399d0fca3038901fbfbfc87c95e04d6c10261df055cc727e094e5903f2d4c97621c1f9b1f68f0c9ccebf" },
                { "vi", "311b111e4d246c25f71e0c2a64aa33f1d9ac922e9b1e5d613fdf7c4afbf2bfd6dcb417906dafc7b712661b61d01ab4940af25430d81251c2d9e6ffeb9a69feef" },
                { "xh", "df06411e9f3b214e87a8ff3876d3349c8803dbafc5559182b8c1cc85118c1c38c9fdd0e67f0a7a3ed4a4b3081e4ca1aa640e01d313940bb976363541baa0ffca" },
                { "zh-CN", "85a05f9f41cf9cb8b936c8786dbf7a3ca7ab696d6a3d049efaafc1403ea69f8b14b8f5f4c2c12b19862a781f35ad1673ba85c5e668b1d5202b5fbf15e4e1ccaa" },
                { "zh-TW", "c11c6052e734ac0127760f91bd943e8e5f981222e4c535fc8fd8260d63f86f7443f3a69b5cec57e7dd0c41dd922e556b04d3fdbfbe2cf70c14d0cb942d4d5b69" }
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
            const string knownVersion = "121.0.1";
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
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                client = null;
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
